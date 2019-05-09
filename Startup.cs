using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace JNCC.Microsite.SAC
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var root = Configuration.GetValue<string>("r");

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseStaticFiles(new StaticFileOptions{
                    FileProvider = new PhysicalFileProvider(Path.Combine(String.IsNullOrWhiteSpace(root) ? Directory.GetCurrentDirectory() : root, "docs/images")),
                    RequestPath = "/images"
                })
                .UseStaticFiles(new StaticFileOptions{
                    FileProvider = new PhysicalFileProvider(Path.Combine(String.IsNullOrWhiteSpace(root) ? Directory.GetCurrentDirectory() : root, "docs/frontend")),
                    RequestPath = "/frontend"
                })
                .UseRequestInterceptorMiddleware();
        }
    }

    public class RequestInterceptorMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestInterceptorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 404)
            {
                if (context.Request.Path.ToString().EndsWith(".html"))
                {
                    context.Response.Redirect("/404.html", false);
                }
                else
                {
                    context.Response.Redirect(context.Request.Path + ".html", false);
                }
            }
        }
    }

    public static class RequestInterceptorMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestInterceptorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestInterceptorMiddleware>();
        }
    }
}