﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Infrastructure
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .WriteTo.Console();

                var logglyConfig = context.Configuration.GetSection("Infrastructure").GetSection("Loggly");

                if (logglyConfig.Exists())
                {
                    loggerConfiguration.WriteTo.Loggly();
                }
                else
                {
                    var seqConfig = context.Configuration.GetSection("Infrastructure").GetSection("Seq");

                    if (seqConfig.Exists())
                    {
                        var seqUrl = seqConfig["Url"];

                        loggerConfiguration.WriteTo.Seq(seqUrl!);
                    }
                }

                var sentryConfig = context.Configuration.GetSection("Sentry");

                if (sentryConfig.Exists())
                {
                    var seqUrl = sentryConfig["Url"];

                    loggerConfiguration.WriteTo.Sentry();
                }

                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });

            return hostBuilder;
        }
    }
}
