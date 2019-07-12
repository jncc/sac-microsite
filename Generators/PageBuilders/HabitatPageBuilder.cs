using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Helpers.Generators;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Helpers.Website;
using Microsoft.Extensions.DependencyInjection;

namespace JNCC.Microsite.SAC.Generators.PageBuilders
{
    public static class HabitatPageBuilder
    {
        
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, GeneratorConfig config, InterestFeature feature)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {   
                var helper = RenderHelper.GetRendererHelper(serviceScope);
                var model = new InterestFeaturePage
                {
                    GeneratorConfig = config,
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/habitat/", "Habitats", true) },
                    CurrentSection = "Habitat",
                    InterestFeature = feature,
                    Title = StringHelpers.RemoveHTMLTags(String.Format("{0} ({1}) - {2}", feature.LayTitle, feature.Name, Page.DefaultTitle)),
                    MetaDescription = String.Format("Habitat account of Habitats Directive feature {0}, {1}. The Habitats Directive: selection of Special Areas of Conservation in the UK, second edition, JNCC (2002)", feature.Code, feature.Name),
                    MetaKeywords = new List<string> {feature.Code, feature.Name}
                };

                return helper.RenderViewToStringAsync("Views/HabitatInterestFeature.cshtml", model);
            }
        }

    }
}
