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
                    habitat.FeatureDescription = Regex.Replace(habitat.FeatureDescription, @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);
                    habitat.EUStatus = Regex.Replace(habitat.EUStatus, @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);                    
                    var habitatPageContent = HabitatPageBuilder.RenderPage(serviceScopeFactory, habitat).Result;

                    using (StreamWriter writer = new StreamWriter(String.Format("output/html/habitat/{0}.html", habitat.Code)))
                    {
                        writer.Write(habitatPageContent);
                    }
                }

                using (StreamWriter writer = new StreamWriter("output/html/habitat/index.html"))
                {
                    var habitatListContent = InterestFeatureListPageBuilder.RenderPage(serviceScopeFactory, true, habitats).Result;
                    writer.Write(habitatListContent);
                }
            }

        }
    }
}