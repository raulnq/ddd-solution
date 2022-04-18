using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                var settings = context.Configuration.GetSection("Infrastructure").GetSection("Seq").Get<SeqSettings>();

                if (settings == null)
                {
                    return;
                }

                loggerConfiguration
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(settings.Url!)
                .ReadFrom.Configuration(context.Configuration);
            });

            return hostBuilder;
        }
    }
}
