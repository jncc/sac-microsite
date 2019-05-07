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

using Mono.Options;
using Newtonsoft.Json;

using JNCC.Microsite.SAC.Data;
using JNCC.Microsite.SAC.Generators;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Website;

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
            string accessDbPath = "";
            bool update = false;
            bool generate = false;
            bool view = false;

            var options = new OptionSet {
                { "a|accessdb=", "path to the Access DB containg SAC info", a => accessDbPath = a},
                { "u|update", "run data update from Database", u => update = true},
                { "g|generate", "generate web pages from extracted data", g => generate = true},
                { "v|view", "view the static web site", v => view = true},
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

                }
                else
                {
                    DatabaseExtractor.ExtractData(accessDbPath);
                }
            }

            if (generate)
            {
                Generator.MakeSite();
            }

            if (view)
            {
                CreateWebHostBuilder(args)
                    .UseStartup<Startup>()
                    .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "output/html"))
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
