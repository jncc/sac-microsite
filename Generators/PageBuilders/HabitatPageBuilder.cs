using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Generators.Helpers;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using Microsoft.Extensions.DependencyInjection;

namespace JNCC.Microsite.SAC.Generators.PageBuilders
{
    public static class HabitatPageBuilder
    {
        
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, InterestFeature feature)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {   
                var helper = RenderHelper.GetRendererHelper(serviceScope);
                var model = new InterestFeaturePage
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/habitat", "Habitats", true), (string.Format("/habitat/{0}", feature.Code), feature.Name, true) },
                    CurrentSection = "Habitat",
                    InterestFeature = feature,
                    Title = String.Format("{0} ({1}) - {2}", feature.LayTitle, feature.Name, Page.DefaultTitle)
                };

                return helper.RenderViewToStringAsync("Views/HabitatInterestFeature.cshtml", model);
            }
        }

    }
}
