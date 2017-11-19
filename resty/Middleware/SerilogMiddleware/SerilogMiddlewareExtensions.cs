using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace resty.Middleware.SerilogMiddleware
{
    public static class SerilogMiddlewareExtensions
    {
        public static IApplicationBuilder UseSerilogMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<SerilogMiddleware>();

            return app;
        }
    }
}
