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
    public static class SpeciesGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory)
        {
            using (StreamReader fileReader = new StreamReader("output/json/species.json"))
            {
                List<InterestFeature> speciesList = JsonConvert.DeserializeObject<List<InterestFeature>>(fileReader.ReadToEnd());

                foreach (var species in speciesList)
                {
                    species.FeatureDescription = Regex.Replace(species.FeatureDescription, @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);
                    species.EUStatus = Regex.Replace(species.EUStatus, @"<(font|\/font|FONT|\/FONT)[^>]{0,}>", string.Empty);
                    var habitatPageContent = SpeciesPageBuilder.RenderPage(serviceScopeFactory, species).Result;

                    using (StreamWriter writer = new StreamWriter(String.Format("output/html/species/{0}.html", species.Code)))
                    {
                        writer.Write(habitatPageContent);
                    }
                }

                using (StreamWriter writer = new StreamWriter("output/html/species/index.html"))
                {
                    var speciesListContent = InterestFeatureListPageBuilder.RenderPage(serviceScopeFactory, false, speciesList).Result;
                    writer.Write(speciesListContent);
                }                
            }
        }
    }
}