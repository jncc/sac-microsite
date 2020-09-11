using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Helpers;

namespace JNCC.Microsite.SAC
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILogger<Startup> log)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.WebRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Webserver Root is at <root>/output/html
            Console.WriteLine("Webserver root is {0}", env.WebRootPath);

            // Static files Root is at <root>/docs
            string staticFilesRoot = FileHelper.GetActualFilePath(env.WebRootPath, Path.Combine("..", "..", "docs"));
            Console.WriteLine("Static files root is {0}", staticFilesRoot);

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseRequestInterceptorMiddleware();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(staticFilesRoot)
            });
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