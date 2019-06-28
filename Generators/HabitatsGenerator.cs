using System;
using System.Collections.Generic;
using System.IO;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using JNCC.Microsite.SAC.Generators.PageBuilders;
using JNCC.Microsite.SAC.Helpers;

namespace JNCC.Microsite.SAC.Generators
{
    public static class HabitatsGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory, GeneratorConfig config, string basePath, bool generateSearchDocuments, string searchIndex, List<SitemapEntry> sitemapEntries)
        {
            using (StreamReader fileReader = new StreamReader(FileHelper.GetActualFilePath(basePath, "output/json/habitats.json")))
            {
                List<InterestFeature> habitats = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var habitat in habitats)
                {
                    var habitatPageContent = HabitatPageBuilder.RenderPage(serviceScopeFactory, config, habitat).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/habitat/{0}/index.html", habitat.Code)), habitatPageContent);
                    sitemapEntries.Add(new SitemapEntry
                    {
                        URL = String.Format("/habitat/{0}/", habitat.Code)
                    });

                    if (generateSearchDocuments)
                    {
                        FileHelper.WriteJSONToFile(
                            String.Format("output/search/habitat/{0}.json", habitat.Code),
                            SearchHelpers.GetHabitatPageSearchDocument(searchIndex, habitat.Code, habitat.Name, habitatPageContent)
                        );
                    }

                    var habitatMapCompareContent = InterestFeatureComparisonPageBuilder.RenderPage(serviceScopeFactory, config, habitat).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/habitat/{0}/comparison.html", habitat.Code)), habitatMapCompareContent);
                    sitemapEntries.Add(new SitemapEntry
                    {
                        URL = String.Format("/habitat/{0}/comparison", habitat.Code)
                    });

                    var habitatMapContent = InterestFeatureMapPageBuilder.RenderPage(serviceScopeFactory, config, habitat).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/habitat/{0}/map.html", habitat.Code)), habitatMapContent);
                    sitemapEntries.Add(new SitemapEntry
                    {
                        URL = String.Format("/habitat/{0}/map", habitat.Code)
                    });

                    var habitatDistributionContent = InterestFeatureDistributionPageBuilder.RenderPage(serviceScopeFactory, config, habitat).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/habitat/{0}/distribution.html", habitat.Code)), habitatDistributionContent);
                    sitemapEntries.Add(new SitemapEntry
                    {
                        URL = String.Format("/habitat/{0}/distribution", habitat.Code)
                    });
                }

                var habitatListContent = InterestFeatureListPageBuilder.RenderPage(serviceScopeFactory, config, true, habitats).Result;
                FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, "output/html/habitat/index.html"), habitatListContent);
                sitemapEntries.Add(new SitemapEntry
                {
                    URL = "/habitat/"
                });

                Console.WriteLine("Generated pages for {0} habitats", habitats.Count);

                if (generateSearchDocuments)
                {
                    Console.WriteLine("Generated search elements for {0} habitats", habitats.Count);
                }
            }

        }
    }
}