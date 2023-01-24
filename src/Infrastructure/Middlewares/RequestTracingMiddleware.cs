using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Middlewares
{
    public class RequestTracingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTracingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("TransactionId", context.TraceIdentifier))
            using (LogContext.PushProperty("Path", $"{context.Request?.Path.Value}{context.Request?.QueryString.Value}"))
            {
                await _next(context);
            }
        }
    }
}
