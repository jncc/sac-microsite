using System;
using System.Collections.Generic;
using System.IO;
using JNCC.Microsite.SAC.Data;
using JNCC.Microsite.SAC.Models.Data;
using Newtonsoft.Json;

namespace JNCC.Microsite.SAC.Data
{
    public static class DatabaseExtractor 
    {
        public static void ExtractData(string accessDbPath) 
        {
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