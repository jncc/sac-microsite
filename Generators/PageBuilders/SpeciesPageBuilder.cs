using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Generators.Helpers;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Website.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace JNCC.Microsite.SAC.Generators.PageBuilders
{
    public static class SpeciesPageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, InterestFeature feature)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                var model = new InterestFeaturePage
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/species", "Species", true), (string.Format("/species/{0}", feature.Code), feature.Name, true) },
                    CurrentSection = "Species",
                    InterestFeature = feature,
                    Title = StringHelpers.RemoveHTMLTags(String.Format("{0} ({1}) - {2}", feature.LayTitle, feature.Name, Page.DefaultTitle))
                };

                return helper.RenderViewToStringAsync("Views/SpeciesInterestFeature.cshtml", model);
            }
        }
    }
}