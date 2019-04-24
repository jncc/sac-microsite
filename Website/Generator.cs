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

namespace JNCC.Microsite.SAC.Website
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

            using (StreamReader fileReader = new StreamReader("output/json/sites.json"))
            {
                List<Site> sites = JsonConvert.DeserializeObject<List<Site>>(fileReader.ReadToEnd());

                var searchPageContent = PageBuilders.RenderSearchPage(serviceScopeFactory, sites.Select(s => (s.EUCode, s.Name))).Result;

                using (StreamWriter writer = new StreamWriter("output/html/index.html"))
                {
                    writer.Write(searchPageContent);
                }

                foreach (var site in sites)
                {
                    var sitePageContent = PageBuilders.RenderSitePage(serviceScopeFactory, site).Result;

                    using (StreamWriter writer = new StreamWriter(String.Format("output/html/site/{0}.html", site.EUCode)))
                    {
                        writer.Write(sitePageContent);
                    }
                }
            }

            using (StreamReader fileReader = new StreamReader("output/json/habitats.json"))
            {
                List<InterestFeature> habitats = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var habitat in habitats) 
                {
                    var habitatPageContent = PageBuilders.RenderHabitatPage(serviceScopeFactory, habitat).Result;

                    using (StreamWriter writer = new StreamWriter(String.Format("output/html/habitat/{0}.html", habitat.Code)))
                    {
                        writer.Write(habitatPageContent);
                    }
                }
            }

            using (StreamReader fileReader = new StreamReader("output/json/species.json"))
            {
                List<InterestFeature> speciesList = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var species in speciesList) 
                {
                    var habitatPageContent = PageBuilders.RenderHabitatPage(serviceScopeFactory, species).Result;

                    using (StreamWriter writer = new StreamWriter(String.Format("output/html/species/{0}.html", species.Code)))
                    {
                        writer.Write(habitatPageContent);
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(String.Format("output/html/404.html")))
            {
                var notFoundContent = PageBuilders.RenderErrorPage(serviceScopeFactory, 404).Result;
                writer.Write(notFoundContent);
            }                
        }
    }
}