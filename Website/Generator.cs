using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/Search", "Search", true) },
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

                // Regional Site Lists
                using (StreamWriter writer = new StreamWriter("output/html/site/index.html"))
                {
                    var siteListPageContent = PageBuilders.RenderSiteListPage(
                        serviceScopeFactory,
                        "Sites in the United Kingdom",
                        String.Format("<p>There are {0} designated SACs, SCIs or cSACs in the <b>United Kingdom</b> including cross border sites (excluding <a href=\"#\">Gibraltar</a>). Cross border sites are listed under both countries. Sites are sorted alphabetically within country.</p>", sites.Count),
                        null,
                        new List<RegionalSites>{
                        new RegionalSites {
                            Region = "England",
                            Sites = sites.Where(s => s.Country.Contains("E"))
                        },
                        new RegionalSites {
                            Region = "Northern Ireland",
                            Sites = sites.Where(s => s.Country.Contains("NI"))
                        },
                        new RegionalSites {
                            Region = "Scotland",
                            Sites = sites.Where(s => s.Country.Contains("S"))
                        },
                        new RegionalSites {
                            Region = "Wales",
                            Sites = sites.Where(s => s.Country.Contains("W"))
                        },
                        new RegionalSites {
                            Region = "Offshore",
                            Sites = sites.Where(s => s.Country.Contains("O"))
                        },
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/england.html"))
                {
                    var siteListPageContent = PageBuilders.RenderSiteListPage(
                        serviceScopeFactory,
                        "SACs in England",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>England</b> including cross border sites. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("E")).Count()),
                        ("/england", "England", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "England",
                                Sites = sites.Where(s => s.Country.Contains("E"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/northern-ireland.html"))
                {
                    var siteListPageContent = PageBuilders.RenderSiteListPage(
                        serviceScopeFactory,
                        "SACs in Northern Ireland",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>Northern Ireland</b> including cross border sites. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("NI")).Count()),
                        ("/northern-ireland", "Northern Ireland", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "Northern Ireland",
                                Sites = sites.Where(s => s.Country.Contains("NI"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/scotland.html"))
                {
                    var siteListPageContent = PageBuilders.RenderSiteListPage(
                        serviceScopeFactory,
                        "SACs in Scotland",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>Scotland</b> including cross border sites. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("S")).Count()),
                        ("/scotland", "Scotland", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "Scotland",
                                Sites = sites.Where(s => s.Country.Contains("S"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/wales.html"))
                {
                    var siteListPageContent = PageBuilders.RenderSiteListPage(
                        serviceScopeFactory,
                        "SACs in Wales",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>Wales</b> including cross border sites. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("S")).Count()),
                        ("/wales", "Wales", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "Wales",
                                Sites = sites.Where(s => s.Country.Contains("W"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/offshore.html"))
                {
                    var siteListPageContent = PageBuilders.RenderSiteListPage(
                        serviceScopeFactory,
                        "SACs in UK offshore waters",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>UK offshore waters including those that cross the 12 mile limit</b>. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("O")).Count()),
                        ("/offshore", "Offshore", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "Offshore",
                                Sites = sites.Where(s => s.Country.Contains("O"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

            }

            using (StreamReader fileReader = new StreamReader("output/json/habitats.json"))
            {
                List<InterestFeature> habitats = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var habitat in habitats)
                {
                    habitat.FeatureDescription = Regex.Replace(habitat.FeatureDescription, @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);
                    habitat.EUStatus = Regex.Replace(habitat.EUStatus, @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);                    
                    var habitatPageContent = PageBuilders.RenderHabitatPage(serviceScopeFactory, habitat).Result;

                    using (StreamWriter writer = new StreamWriter(String.Format("output/html/habitat/{0}.html", habitat.Code)))
                    {
                        writer.Write(habitatPageContent);
                    }
                }

                using (StreamWriter writer = new StreamWriter("output/html/habitat/index.html"))
                {
                    var habitatListContent = PageBuilders.RenderInterestFeatureListPage(serviceScopeFactory, true, habitats).Result;
                    writer.Write(habitatListContent);
                }
            }

            using (StreamReader fileReader = new StreamReader("output/json/species.json"))
            {
                List<InterestFeature> speciesList = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var species in speciesList)
                {
                    species.FeatureDescription = Regex.Replace(species.FeatureDescription, @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);
                    species.EUStatus = Regex.Replace(species.EUStatus, @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);
                    var habitatPageContent = PageBuilders.RenderSpeciesPage(serviceScopeFactory, species).Result;

                    using (StreamWriter writer = new StreamWriter(String.Format("output/html/species/{0}.html", species.Code)))
                    {
                        writer.Write(habitatPageContent);
                    }
                }

                using (StreamWriter writer = new StreamWriter("output/html/species/index.html"))
                {
                    var speciesListContent = PageBuilders.RenderInterestFeatureListPage(serviceScopeFactory, false, speciesList).Result;
                    writer.Write(speciesListContent);
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