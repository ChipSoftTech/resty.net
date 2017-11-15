using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using AppWithScheduler.Code.Scheduling;
using AppWithScheduler.Code;
using CST.NETCore.Headers;

namespace resty
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Distributed Cache, can use memory, sql, redis, ...
            services.AddDistributedMemoryCache();

            //Compression middleware
            services.AddResponseCompression();

            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // Add service scheduled tasks & scheduler
            //services.AddSingleton<IScheduledTask, QuoteOfTheDayTask>();
            services.AddSingleton<IScheduledTask, CacheWriteTask>();
            services.AddScheduler((sender, args) =>
            {
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error.html");
            }

            //Add security and custom headers
            app.UseSecurityHeadersMiddleware(
                new SecurityHeadersBuilder()
                    .AddDefaultSecurePolicy()
                    .AddCustomHeader("Content-Type", "application/json"));

            // Cors global policy - assign here or on each controller
            app.UseCors("CorsPolicy");

            //Static files
            app.UseFileServer();

            //Compression Middleware
            app.UseResponseCompression();

            //Storage Middleware logic and processing
            //Processes the request & response
            app.UseStoreMiddleware();

        }
    }
}
