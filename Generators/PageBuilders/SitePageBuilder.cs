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
    public static class SitePageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, GeneratorConfig config, Site site)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                var model = new SitePage
                {
                    GeneratorConfig = config,
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/site/", "Sites", true), (String.Format("/site/{0}", site.EUCode), site.Name, true) },
                    CurrentSection = "Site",
                    Site = site,
                    Title = StringHelpers.RemoveHTMLTags(String.Format("{0} - Special Areas of Conservation", site.Name)),
                    MetaDescription = String.Format("SAC selection criteria for site %s, EU Code %s, Unitary Authority %s, %s", StringHelpers.RemoveHTMLTags(site.Name), site.EUCode, site.LocalAuthority, site.CountryFull),
                    MetaKeywords = new List<string> {site.Name, site.EUCode, site.LocalAuthority}
                };

                return helper.RenderViewToStringAsync("Views/Site.cshtml", model);
            }
        }
    }
}