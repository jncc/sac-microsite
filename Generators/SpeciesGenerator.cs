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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using JNCC.Microsite.SAC.Generators.PageBuilders;
using JNCC.Microsite.SAC.Helpers;
using JNCC.Microsite.SAC.Helpers.Generators;

namespace JNCC.Microsite.SAC.Generators
{
    public static class SpeciesGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory, GeneratorConfig config, string basePath, bool generateSearchDocuments, string searchIndex)
        {
            using (StreamReader fileReader = new StreamReader(FileHelper.GetActualFilePath(basePath, "output/json/species.json")))
            {
                List<InterestFeature> speciesList = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var species in speciesList)
                {
                    var speciesPageContent = SpeciesPageBuilder.RenderPage(serviceScopeFactory, config, species).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/species/{0}/index.html", species.Code)), speciesPageContent);

                    if (generateSearchDocuments)
                    {
                        FileHelper.WriteJSONToFile(
                            String.Format("output/search/species/{0}.json", species.Code),
                            SearchHelpers.GetSpeciesPageSearchDocument(searchIndex, species.Code, species.Name, speciesPageContent)
                        );
                    }

                    var speciesMapCompareContent = InterestFeatureComparisonPageBuilder.RenderPage(serviceScopeFactory, config, species).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/species/{0}/comparison.html", species.Code)), speciesMapCompareContent);

                    var speciesMapContent = InterestFeatureMapPageBuilder.RenderPage(serviceScopeFactory, config, species).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/species/{0}/map.html", species.Code)), speciesMapContent);

                    var speciesDistributionContent = InterestFeatureDistributionPageBuilder.RenderPage(serviceScopeFactory, config, species).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/species/{0}/distribution.html", species.Code)), speciesDistributionContent);
                }

                var speciesListContent = InterestFeatureListPageBuilder.RenderPage(serviceScopeFactory, config, false, speciesList).Result;
                FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, "output/html/species/index.html"), speciesListContent);

                Console.WriteLine("Generated pages for {0} species", speciesList.Count);

                if (generateSearchDocuments)
                {
                    Console.WriteLine("Generated search elements for {0} species", speciesList.Count);
                }
            }
        }
    }
}