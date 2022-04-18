using Microsoft.Extensions.Configuration;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Infrastructure
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("es")
            };

            return app
                    .UseRequestLocalization(new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("en-US"),
                        SupportedCultures = supportedCultures,
                        SupportedUICultures = supportedCultures
                    });
        }

        public static IApplicationBuilder UseProblemDetails(this IApplicationBuilder app) =>
            ProblemDetailsExtensions.UseProblemDetails(app);

        public static IApplicationBuilder UseCors(this IApplicationBuilder app, IConfiguration configuration)
        {
            var corsSection = configuration.GetSection("Cors");

            if (corsSection == null)
            {
                return app;
            }

            foreach (var item in corsSection.GetChildren())
            {
                app.UseCors(item.Key);
            }

            return app;
        }

        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app) => app.UseHealthChecks("/health");
    }
}
