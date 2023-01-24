using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure
{
    public class UserTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ClaimTypesSessionId = "http://schemas.microsoft.com/ws/2008/06/identity/claims/sessionid";
        public UserTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                telemetry.Context.User.AuthenticatedUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
                telemetry.Context.User.Id = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                telemetry.Context.Session.Id = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypesSessionId);
                telemetry.Context.Device.Type = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.System);
                telemetry.Context.Component.Version = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Version);
            }
        }
    }
}
