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
    public static class HabitatsGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory, GeneratorConfig config, string basePath, bool generateSearchDocuments, string searchIndex)
        {
            using (StreamReader fileReader = new StreamReader(FileHelper.GetActualFilePath(basePath, "output/json/habitats.json")))
            {
                List<InterestFeature> habitats = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var habitat in habitats)
                {
                    var habitatPageContent = HabitatPageBuilder.RenderPage(serviceScopeFactory, config, habitat).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/habitat/{0}/index.html", habitat.Code)), habitatPageContent);

                    if (generateSearchDocuments)
                    {
                        FileHelper.WriteJSONToFile(
                            String.Format("output/search/habitat/{0}.json", habitat.Code),
                            SearchHelpers.GetHabitatPageSearchDocument(searchIndex, habitat.Code, habitat.Name, habitatPageContent)
                        );
                    }

                    var habitatMapCompareContent = InterestFeatureComparisonPageBuilder.RenderPage(serviceScopeFactory, config, habitat).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/habitat/{0}/comparison.html", habitat.Code)), habitatMapCompareContent);

                    var habitatMapContent = InterestFeatureMapPageBuilder.RenderPage(serviceScopeFactory, config, habitat).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/habitat/{0}/map.html", habitat.Code)), habitatMapContent);

                    var habitatDistributionContent = InterestFeatureDistributionPageBuilder.RenderPage(serviceScopeFactory, config, habitat).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/habitat/{0}/distribution.html", habitat.Code)), habitatDistributionContent);
                }

                var habitatListContent = InterestFeatureListPageBuilder.RenderPage(serviceScopeFactory, config, true, habitats).Result;
                FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, "output/html/habitat/index.html"), habitatListContent);

                Console.WriteLine("Generated pages for {0} habitats", habitats.Count);

                if (generateSearchDocuments)
                {
                    Console.WriteLine("Generated search elements for {0} habitats", habitats.Count);
                }
            }

        }
    }
}