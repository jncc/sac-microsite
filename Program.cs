using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using Newtonsoft.Json;
using JNCC.Microsite.SAC.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Models.Data;
using Mono.Options;

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

        static void Main(string[] args)
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
        }
    }
}
