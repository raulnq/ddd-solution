using Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Execution;
using System.Reflection;
using AutoMapper;
using Events;
using Rebus.Handlers;
using MediatR;
using static Rebus.Routing.TypeBased.TypeBasedRouterConfigurationExtensions;
using Serilog;
using Rebus.Config;
using Rebus.Serialization.Json;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandDispatcher<TEvent, TCommand>(this IServiceCollection services, Action<IMappingExpression<TEvent, TCommand>>? mapper = null,
            Func<TEvent, bool>? when = null, Action<MapperConfigurationExpression>? mapperOptions = null)
            where TEvent : IEvent
            where TCommand : ICommand
        {
            if (mapper != null)
            {
                services.PostConfigure<MapperConfigurationExpression>((options) =>
                {
                    mapperOptions?.Invoke(options);

                    mapper(options.CreateMap<TEvent, TCommand>());
                });
            }
            else
            {
                services.PostConfigure<MapperConfigurationExpression>((options) =>
                {
                    mapperOptions?.Invoke(options);

                    options.CreateMap<TEvent, TCommand>();
                });
            }

            return services.AddTransient<IHandleMessages<TEvent>, CommandDispatcher<TEvent, TCommand>>(provider => new CommandDispatcher<TEvent, TCommand>(provider.GetRequiredService<IMediator>(), provider.GetRequiredService<IMapper>(), when));
        }

        public static IServiceCollection AddCommandDispatcher<TCommand>(this IServiceCollection services)
            where TCommand : ICommand
        {
            return services.AddTransient<IHandleMessages<TCommand>, CommandDispatcher<TCommand>>();
        }

        public static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration, Assembly assembly, Action<RebusSettings, TypeBasedRouterConfigurationBuilder>? map = null)
        {
            var settings = configuration.GetSection("RebusSettings").Get<RebusSettings>();

            if (settings == null)
            {
                return services;
            }

            services.AutoRegisterHandlersFromAssembly(assembly);

            services.AddSingleton(settings);

            services.AddSingleton<RebusEventPublisher>();

            services.AddSingleton<IEventPublisher, RebusEventPublisher>(provider => provider.GetRequiredService<RebusEventPublisher>());

            services.AddSingleton<ICommandSender, RebusCommandSender>();

            services.AddHostedService<EventPublisherBackgroundService>();

            var logger = Log.ForContext("queue", settings.Queue);

            services
                .AddRebus(configurer =>
                {
                    return configurer
                        .Logging(l => l.Serilog(logger))
                        .Serialization(s => s.UseNewtonsoftJson(JsonInteroperabilityMode.PureJson))
                        .Transport(t =>
                        {
                            t.UseRabbitMq(settings.ConnectionString, settings.Queue);
                        })
                        .Routing(r =>
                        {
                            if (map != null)
                            {
                                map(settings, r.TypeBased());
                            }
                            else
                            {
                                r.TypeBased();
                            }
                        })
                        .Options(o =>
                        {
                            o.Decorate<IPipeline>(c =>
                            {
                                var pipeline = c.Get<IPipeline>();
                                var stepToInject = new MessageTracingStep();

                                return new PipelineStepInjector(pipeline)
                                    .OnReceive(stepToInject, PipelineRelativePosition.Before, typeof(DispatchIncomingMessageStep));
                            });
                        });
                });

            return services;
        }
    }
}
