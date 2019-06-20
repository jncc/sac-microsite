using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Helpers.Generators;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using Microsoft.Extensions.DependencyInjection;
using JNCC.Microsite.SAC.Helpers.Website;

namespace JNCC.Microsite.SAC.Generators.PageBuilders
{
    public static class InterestFeatureComparisonPageBuilder
    {

        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, GeneratorConfig config, InterestFeature feature)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                var isHabitat = InterestFeatureHelpers.IsHabitatCode(feature.Code);

                var breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true) };
                
                if (isHabitat)
                {
                    breadcrumbs.Add(("/habitat/", "Habitats", true));
                    breadcrumbs.Add((string.Format("/habitat/{0}", feature.Code), feature.Name, true));
                    breadcrumbs.Add((string.Format("/habitat/{0}/comparison", feature.Code), "Comparison", true));
                }
                else
                {
                    breadcrumbs.Add(("/species/", "Species", true));
                    breadcrumbs.Add((string.Format("/species/{0}", feature.Code), feature.Name, true));
                    breadcrumbs.Add((string.Format("/species/{0}/comparison", feature.Code), "Comparison", true));
                }

                var model = new InterestFeaturePage
                {
                    GeneratorConfig = config,
                    Breadcrumbs = breadcrumbs,
                    CurrentSection = isHabitat ? "Habitat" : "Species",
                    InterestFeature = feature,
                    Title = StringHelpers.RemoveHTMLTags(String.Format("{0} ({1}) Distribution Comparison - {2}", feature.LayTitle, feature.Name, Page.DefaultTitle)),
                    MetaDescription = String.Format("A distribution comparison of Habitats Directive feature %s, %s. The Habitats Directive: selection of Special Areas of Conservation in the UK, second edition, JNCC (2002)", feature.Code, feature.Name),
                    MetaKeywords = new List<string> {feature.Code, feature.Name}
                };

                return helper.RenderViewToStringAsync("Views/InterestFeatureMapCompare.cshtml", model);
            }
        }
    }
}
