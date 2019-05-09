using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Generators.Helpers;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using Microsoft.Extensions.DependencyInjection;
using JNCC.Microsite.SAC.Website.Helpers;
using System;

namespace JNCC.Microsite.SAC.Generators.PageBuilders
{
    public static class InterestFeatureDistributionPageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, InterestFeature feature)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                var isHabitat = InterestFeatureHelpers.IsHabitatCode(feature.Code);

                var breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true) };
                
                if (isHabitat)
                {
                    breadcrumbs.Add(("/habitat", "Habitats", true));
                    breadcrumbs.Add((string.Format("/habitat/{0}", feature.Code), feature.Name, true));
                    breadcrumbs.Add((string.Format("/habitat/{0}/distribution", feature.Code), "Distribution", true));
                }
                else
                {
                    breadcrumbs.Add(("/species", "Species", true));
                    breadcrumbs.Add((string.Format("/species/{0}", feature.Code), feature.Name, true));
                    breadcrumbs.Add((string.Format("/species/{0}/distribution", feature.Code), "Distribution", true));
                }

                var model = new InterestFeaturePage
                {
                    Breadcrumbs = breadcrumbs,
                    CurrentSection = isHabitat ? "Habitat" : "Species",
                    InterestFeature = feature
                };

                return helper.RenderViewToStringAsync("Views/InterestFeatureDistribution.cshtml", model);
            }
        }
    }
}