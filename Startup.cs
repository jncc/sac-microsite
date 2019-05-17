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
using JNCC.Microsite.SAC.Helpers;
using JNCC.Microsite.SAC.Helpers.Runtime;

namespace JNCC.Microsite.SAC
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILogger<Startup> log)
        {
            log.LogDebug("Started config build");
            
            Console.WriteLine("starting config build");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.WebRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Console.WriteLine("ending config build");
            log.LogDebug("Finished config build");
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Console.WriteLine("Webserver root is {0}", env.WebRootPath);

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseRequestInterceptorMiddleware();

            try
            {
                app.UseStaticFiles(new StaticFileOptions{
                    FileProvider = new PhysicalFileProvider(FileHelper.GetActualFilePath(env.WebRootPath, "images")),
                    RequestPath = "/images"
                });   
            }
            catch (DirectoryNotFoundException)
            {
                
                throw new DirectoryNotFoundException("Images folder not fouind in <root>/docs. The static images folder must be placed in the <output root>/docs folder before the site can be generated");
            }

            try
            {
                app.UseStaticFiles(new StaticFileOptions{
                    FileProvider = new PhysicalFileProvider(FileHelper.GetActualFilePath(env.WebRootPath, "frontend")),
                    RequestPath = "/frontend"
                });
            }
            catch (DirectoryNotFoundException)
            {
                
                throw new DirectoryNotFoundException("Frontend folder not found in <root>/docs. The static frontend folder must be placed in the <output root>/docs folder before the site can be generated");
            }
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