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
    public static class SpeciesGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory, GeneratorConfig config, string basePath, bool generateSearchDocuments, string searchIndex, List<SitemapEntry> sitemapEntries)
        {
            using (StreamReader fileReader = new StreamReader(FileHelper.GetActualFilePath(basePath, "output/json/species.json")))
            {
                List<InterestFeature> speciesList = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var species in speciesList)
                {
                    var speciesPageContent = SpeciesPageBuilder.RenderPage(serviceScopeFactory, config, species).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/species/{0}/index.html", species.Code)), speciesPageContent);

                    sitemapEntries.Add(new SitemapEntry
                    {
                        URL = String.Format("/species/{0}/", species.Code)
                    });

                    if (generateSearchDocuments)
                    {
                        FileHelper.WriteJSONToFile(
                            String.Format("output/search/species/{0}.json", species.Code),
                            SearchHelpers.GetSpeciesPageSearchDocument(searchIndex, species.Code, String.Format("{0} [{1}]", species.LayTitle, species.Name), speciesPageContent)
                        );
                    }
                    
                    // don't generate comparrison pages

                    // var speciesMapCompareContent = InterestFeatureComparisonPageBuilder.RenderPage(serviceScopeFactory, config, species).Result;
                    // FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/species/{0}/comparison.html", species.Code)), speciesMapCompareContent);
                    // sitemapEntries.Add(new SitemapEntry
                    // {
                    //     URL = String.Format("/species/{0}/comparison", species.Code)
                    // });

                    var speciesMapContent = InterestFeatureMapPageBuilder.RenderPage(serviceScopeFactory, config, species).Result;
                    FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/species/{0}/map.html", species.Code)), speciesMapContent);
                    sitemapEntries.Add(new SitemapEntry
                    {
                        URL = String.Format("/species/{0}/map", species.Code)
                    });

                    // don't generate distribution pages

                    // var speciesDistributionContent = InterestFeatureDistributionPageBuilder.RenderPage(serviceScopeFactory, config, species).Result;
                    // FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, String.Format("output/html/species/{0}/distribution.html", species.Code)), speciesDistributionContent);
                    // sitemapEntries.Add(new SitemapEntry
                    // {
                    //     URL = String.Format("/species/{0}/distribution", species.Code)
                    // });
                }

                var speciesListContent = InterestFeatureListPageBuilder.RenderPage(serviceScopeFactory, config, false, speciesList).Result;
                FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, "output/html/species/index.html"), speciesListContent);
                sitemapEntries.Add(new SitemapEntry
                {
                    URL = "/species/"
                });

                Console.WriteLine("Generated pages for {0} species", speciesList.Count);

                if (generateSearchDocuments)
                {
                    Console.WriteLine("Generated search elements for {0} species", speciesList.Count);
                }
            }
        }
    }
}