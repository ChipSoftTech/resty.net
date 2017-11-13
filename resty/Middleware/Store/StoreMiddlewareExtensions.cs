using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace resty
{
    public static class StoreMiddlewareExtensions
    {
        public static IApplicationBuilder UseStoreMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<StoreMiddleware>();

            return app;
        }
    }
}
