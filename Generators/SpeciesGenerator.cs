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
using JNCC.Microsite.SAC.Generators.Helpers;

namespace JNCC.Microsite.SAC.Generators
{
    public static class SpeciesGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory)
        {
            using (StreamReader fileReader = new StreamReader("output/json/species.json"))
            {
                List<InterestFeature> speciesList = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var species in speciesList)
                {
                    var habitatPageContent = SpeciesPageBuilder.RenderPage(serviceScopeFactory, species).Result;
                    RenderHelper.WriteToFile(String.Format("output/html/species/{0}/index.html", species.Code), habitatPageContent);

                    var speciesMapCompareContent = InterestFeatureComparisonPageBuilder.RenderPage(serviceScopeFactory, species).Result;
                    RenderHelper.WriteToFile(String.Format("output/html/species/{0}/comparison.html", species.Code), speciesMapCompareContent);

                    var speciesMapContent = InterestFeatureMapPageBuilder.RenderPage(serviceScopeFactory, species).Result;
                    RenderHelper.WriteToFile(String.Format("output/html/species/{0}/map.html", species.Code), speciesMapContent);
                }

                var speciesListContent = InterestFeatureListPageBuilder.RenderPage(serviceScopeFactory, false, speciesList).Result;
                RenderHelper.WriteToFile("output/html/species/index.html", speciesListContent);
            }
        }
    }
}