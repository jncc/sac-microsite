using System;
using System.Collections.Generic;
using System.IO;
using JNCC.Microsite.SAC.Data;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Helpers;
using Newtonsoft.Json;

namespace JNCC.Microsite.SAC.Data
{
    public static class DatabaseExtractor 
    {
        public static void ExtractData(string accessDbPath, string outputRoot) 
        {
            Console.WriteLine(String.Format("Updating data files using: {0}", accessDbPath));
            
            var outputBasePath = "output/json";
            
            DatabaseOperations dbOps = new DatabaseOperations(accessDbPath);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;

            Console.WriteLine("Extracting main SAC list");
            List<Site> sites = dbOps.GetFullSACList();
            FileHelper.WriteJSONToFile(FileHelper.GetActualFilePath(outputRoot, outputBasePath, "sites.json"), sites);
            Console.WriteLine(String.Format("Extracted {0} SAC sites", sites.Count));

            Console.WriteLine("Extracting habitat information feature list");
            List<InterestFeature> habitats = dbOps.GetHabitatInformationFeatureList();
            FileHelper.WriteJSONToFile(FileHelper.GetActualFilePath(outputRoot, outputBasePath, "habitats.json"), habitats);
            Console.WriteLine(String.Format("Extracted {0} Habitat Information Features", habitats.Count));

            Console.WriteLine("Extracting species information feature list");
            List<InterestFeature> species = dbOps.GetSpeciesInformationFeatureList();
            FileHelper.WriteJSONToFile(FileHelper.GetActualFilePath(outputRoot, outputBasePath, "species.json"), species);
            Console.WriteLine(String.Format("Extracted {0} Species Information Features", species.Count));
        }
    }
}