using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistance<TDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TDbContext : DbContext, IUnitOfWork, IDomainEventSource, ISequence
        {
            var dbSchema = configuration.GetValue<string>("DbSchema") ?? "dbo";

            services.AddSingleton(_ => new DbSchema(dbSchema));

            services.AddDbContext<TDbContext>(options => options.UseSqlServer(configuration["DbConnectionString"])
                .ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS)));

            services.AddScoped<ISequence, TDbContext>(provider => provider.GetRequiredService<TDbContext>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<TDbContext>());

            services.AddScoped<IDomainEventSource>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>());

            return services;
        }
    }
}
