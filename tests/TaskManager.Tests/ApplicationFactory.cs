using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Domain;
using Tests;

namespace TaskManager.Tests
{
    internal class ApplicationFactory : WebApplicationFactory<Program>, IHttpClienFactory
    {
        public ApplicationFactory()
        {
        }

        public FixedClock Clock { get; set; } = null!;

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(configBuilder =>
            {
                var additionalConfig = new Dictionary<string, string>() { };

                configBuilder.AddInMemoryCollection(additionalConfig!);
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IClock>();
                services.AddSingleton<IClock>(Clock);
            });
        }
    }
}