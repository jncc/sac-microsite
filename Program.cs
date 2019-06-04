using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Logging;

using Mono.Options;
using Newtonsoft.Json;

using JNCC.Microsite.SAC.Data;
using JNCC.Microsite.SAC.Generators;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Helpers;
using JNCC.Microsite.SAC.Helpers.Runtime;

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

        public static void Main(string[] args)
        {
            var showHelp = false;
            string accessDbPath = null;
            bool update = false;
            bool generate = false;
            bool generateSearchDocuments = false;
            string searchIndex = null;
            bool view = false;
            string root = null;
            GeneratorConfig generatorConfig = new GeneratorConfig();

            var options = new OptionSet {
                { "u|update=", "run data update from Database and generate outputs", u => {update = true; accessDbPath = u;}},
                { "g|generate", "generate web pages from extracted data", g => generate = true},
                { "a|analytics", "enable google analytics code in pages", a => generatorConfig.EnableAnalytics = true},
                { "v|view", "view the static web site", v => view = true},
                { "r|root=", "the root path on which to run the generate and view processes", r => root = r},
                { "s|search=", "the search index to generate index documents for", s => {generateSearchDocuments = true; searchIndex = s;}},
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

            if (string.IsNullOrEmpty(root))
            {
                root = Environment.CurrentDirectory; 
            }

            Console.WriteLine("Root path set to {0}", root);

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            if (update)
            {
                if (String.IsNullOrWhiteSpace(accessDbPath))
                {
                    Console.Write("-u | --update option must have a non blank value");

                }
                else
                {
                    DatabaseExtractor.ExtractData(accessDbPath, root);
                }
            }

            if (generate || generateSearchDocuments)
            {
                Generator.MakeSite(generatorConfig, root, generateSearchDocuments, searchIndex);
            }

            if (view)
            {
                Console.WriteLine("Starting Webserver:");

                string webRoot = Path.Combine(root, "output/html");

                CreateWebHostBuilder(args)
                    .UseWebRoot(webRoot)
                    .UseContentRoot(webRoot)
                    .ConfigureLogging((hostingContext, logging) =>  
                    {  
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));  
                        logging.AddConsole();  
                        logging.AddDebug();  
                    })  
                    .UseStartup<Startup>()
                    .Build()
                    .Run();
            }
        }

        static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args);
        }
    }
}
