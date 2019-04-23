using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Renderers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;

namespace JNCC.Microsite.SAC.Website.Helpers
{
    public static class Generator
    {
        //This needs a bit more thought.

        /// Original code from https://github.com/aspnet/Entropy/tree/master/samples/Mvc.RenderViewToString
        private static void ConfigureDefaultServices(IServiceCollection services, string customApplicationBasePath)
        {
            string applicationName;
            IFileProvider fileProvider;
            if (!string.IsNullOrEmpty(customApplicationBasePath))
            {
                applicationName = Path.GetFileName(customApplicationBasePath);
                fileProvider = new PhysicalFileProvider(customApplicationBasePath);
            }
            else
            {
                applicationName = Assembly.GetEntryAssembly().GetName().Name;
                fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            }

            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
            {
                ApplicationName = applicationName,
                WebRootFileProvider = fileProvider,
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(fileProvider);
            });
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddMvc();
            services.AddTransient<RazorViewToStringRenderer>();
        }
        
        /// Original code from https://github.com/aspnet/Entropy/tree/master/samples/Mvc.RenderViewToString
        private static IServiceScopeFactory InitializeServices(string customApplicationBasePath = null)
        {
            // Initialize the necessary services
            var services = new ServiceCollection();
            ConfigureDefaultServices(services, customApplicationBasePath);

            // Add a custom service that is used in the view.
            //services.AddSingleton<EmailReportGenerator>();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }


        private static Task<string> RenderViewSearch(IServiceScopeFactory scopeFactory, IEnumerable<(string EUCode, string Name)> sites)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();

                var model = new Search
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> {("/Search","Search",true)},
                    CurrentSection = "Search",
                    Sites = sites.ToList()
                };

                return helper.RenderViewToStringAsync("Views/Search.cshtml", model);
            }
        }

        public static void MakeSite()
        {
            var serviceScopeFactory = InitializeServices();

            using (StreamReader fileReader = new StreamReader("output/json/sites.json")) {
                List<Site> sites = JsonConvert.DeserializeObject<List<Site>>(fileReader.ReadToEnd());
                var searchPageContent = RenderViewSearch(serviceScopeFactory, sites.Select(s => (s.EUCode, s.Name))).Result;

                Console.WriteLine(searchPageContent);
            }
        }
    }
}