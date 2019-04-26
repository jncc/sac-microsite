using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Generators.Helpers;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using Microsoft.Extensions.DependencyInjection;

namespace JNCC.Microsite.SAC.Generators.PageBuilders
{
    public static class InterestFeatureComparisonPageBuilder
    {
        
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, InterestFeature feature)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {   
                var helper = RenderHelper.GetRendererHelper(serviceScope);
                var model = new InterestFeaturePage
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/habitat", "Habitats", true), (string.Format("/habitat/{0}", feature.Code), feature.Name, true), (string.Format("/habitat/{0}/comparison", feature.Code), "Comparison", true) },
                    CurrentSection = "Habitat",
                    InterestFeature = feature
                };

                return helper.RenderViewToStringAsync("Views/InterestFeatureMapCompare.cshtml", model);
            }
        }
    }
}
