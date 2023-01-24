﻿using Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Execution;
using System.Reflection;
using SqlKata.Compilers;
using Microsoft.Data.SqlClient;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryRunners(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.AddTransient<QueryFactory>(_ =>
            {
                var connection = new SqlConnection(configuration["DbConnectionString"]);

                var compiler = new SqlServerCompiler() { UseLegacyPagination = false };

                var factory = new QueryFactory(connection, compiler);

                return factory;
            });

            var types = assembly.DefinedTypes.Where(type => typeof(IQueryRunner).IsAssignableFrom(type) && type.AsType() != typeof(IQueryRunner));

            foreach (var implementationType in types)
            {
                var genericServiceType = typeof(IQueryRunner<,>);

                var interfaceType = implementationType.ImplementedInterfaces.First(t => t != typeof(IQueryRunner));

                var genericArguments = interfaceType.GenericTypeArguments;

                var serviceType = genericServiceType.MakeGenericType(genericArguments);

                var descriptor = ServiceDescriptor.Describe(serviceType, implementationType, ServiceLifetime.Transient);

                services.Add(descriptor);
            }

            return services;
        }
    }
}
