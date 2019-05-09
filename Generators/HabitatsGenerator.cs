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
    public static class HabitatsGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory)
        {
            using (StreamReader fileReader = new StreamReader("output/json/habitats.json"))
            {
                List<InterestFeature> habitats = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var habitat in habitats)
                {                    
                    var habitatPageContent = HabitatPageBuilder.RenderPage(serviceScopeFactory, habitat).Result;
                    RenderHelper.WriteToFile(String.Format("output/html/habitat/{0}/index.html", habitat.Code), habitatPageContent);

                    var habitatMapCompareContent = InterestFeatureComparisonPageBuilder.RenderPage(serviceScopeFactory, habitat).Result;
                    RenderHelper.WriteToFile(String.Format("output/html/habitat/{0}/comparison.html", habitat.Code), habitatMapCompareContent);

                    var habitatMapContent = InterestFeatureMapPageBuilder.RenderPage(serviceScopeFactory, habitat).Result;
                    RenderHelper.WriteToFile(String.Format("output/html/habitat/{0}/map.html", habitat.Code), habitatMapContent);

                    var habitatDistributionContent = InterestFeatureDistributionPageBuilder.RenderPage(serviceScopeFactory, habitat).Result;
                    RenderHelper.WriteToFile(String.Format("output/html/habitat/{0}/distribution.html", habitat.Code), habitatDistributionContent);
                }

                var habitatListContent = InterestFeatureListPageBuilder.RenderPage(serviceScopeFactory, true, habitats).Result;
                RenderHelper.WriteToFile("output/html/habitat/index.html", habitatListContent);
            }

        }
    }
}