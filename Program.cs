using System.Data.Odbc;
using System.Linq;
using Newtonsoft.Json;
using JNCC.Microsite.SAC.Data;
using JNCC.Microsite.SAC.Models.Data;
using Mono.Options;
using JNCC.Microsite.SAC.Website;
using System.Diagnostics;

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;

using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Renderers;


namespace JNCC.Microsite.SAC
{
    class Program
    {
        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: dotnet run -- [OPTIONS]+");
            Console.WriteLine("Regenerates the JNCC SAC microsite from a given access db and displays a locally hosted testing copy");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

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
        public static IServiceScopeFactory InitializeServices(string customApplicationBasePath = null)
        {
            // Initialize the necessary services
            var services = new ServiceCollection();
            ConfigureDefaultServices(services, customApplicationBasePath);

            // Add a custom service that is used in the view.
            //services.AddSingleton<EmailReportGenerator>();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public static void Main(string[] args)
        {
            var showHelp = false;
            string accessDbPath = "";
            bool update = false;

            var options = new OptionSet {
                { "a|accessdb=", "path to the Access DB containg SAC info", a => accessDbPath = a},
                { "u|update", "run data update from Database", u => update = true},
                { "h|help", "show this message and exit", h => showHelp = h != null }
            };

            List<string> arguments;

            try
            {
                arguments = options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.Write("JNCC.Micosite.SAC: ");
                Console.Write(ex.Message);
                Console.Write("Try `dotnet run -- -h` for more information");
            }

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            if (update)
            {
                if (String.IsNullOrWhiteSpace(accessDbPath))
                {
                    Console.Write("-a | --accessdb option must not be blank if running with -u | --update");
                    return;
                }

                Console.WriteLine(String.Format("Updating data files using: {0}", accessDbPath));

                DatabaseOperations dbOps = new DatabaseOperations(accessDbPath);
                JsonSerializer serializer = new JsonSerializer();

                Console.WriteLine("Extracting main SAC list");
                List<Site> sites = dbOps.GetFullSACList();
                using (StreamWriter sw = new StreamWriter("./output/json/sites.json"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, sites);
                }
                Console.WriteLine(String.Format("Extracted {0} SAC sites", sites.Count));

                Console.WriteLine("Extracting habitat information feature list");
                List<InterestFeature> habitats = dbOps.GetHabitatInformationFeatureList();
                using (StreamWriter sw = new StreamWriter("./output/json/habitats.json"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, habitats);
                }
                Console.WriteLine(String.Format("Extracted {0} Habitat Information Features", habitats.Count));

                Console.WriteLine("Extracting species information feature list");
                List<InterestFeature> species = dbOps.GetSpeciesInformationFeatureList();
                using (StreamWriter sw = new StreamWriter("./output/json/species.json"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, species);
                }
                Console.WriteLine(String.Format("Extracted {0} Species Information Features", species.Count));
            }

            var serviceScopeFactory = InitializeServices();

            using (StreamReader fileReader = new StreamReader("output/json/sites.json"))
            {
                List<Site> sites = JsonConvert.DeserializeObject<List<Site>>(fileReader.ReadToEnd());

                var searchPageContent = PageBuilders.RenderSearchPage(serviceScopeFactory, sites.Select(s => (s.EUCode, s.Name))).Result;

                using (StreamWriter writer = new StreamWriter("output/html/search.html"))
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
        }
    }
}
