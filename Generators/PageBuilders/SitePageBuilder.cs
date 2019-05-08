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
    public static class SitePageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, Site site)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                var model = new SitePage
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/site", "Sites", true), (String.Format("/site/{0}", site.EUCode), site.Name, true) },
                    CurrentSection = "Site",
                    Site = site,
                    Title = StringHelpers.RemoveHTMLTags(String.Format("{0} - Special Areas of Conservation", site.Name))
                };

                return helper.RenderViewToStringAsync("Views/Site.cshtml", model);
            }
        }
    }
}