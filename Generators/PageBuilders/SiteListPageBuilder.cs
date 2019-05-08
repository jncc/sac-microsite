using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Generators.Helpers;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Website.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace JNCC.Microsite.SAC.Generators.PageBuilders
{
    public static class SiteListPageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, string header, string subject, (string href, string text, bool display)? breadcrumb, List<RegionalSites> sites)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);
                
                var breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/site", "Sites", true)};
                if (breadcrumb != null) {
                    breadcrumbs.Add(breadcrumb.Value);
                }

                return helper.RenderViewToStringAsync("Views/SiteList.cshtml", new SiteList
                {
                    Breadcrumbs = breadcrumbs,
                    CurrentSection = "Site",
                    HeaderText = header,
                    SubjectHTML = subject,
                    RegionalSites = sites,
                    Title = StringHelpers.RemoveHTMLTags(String.Format("List of {0} - {1}", header, Page.DefaultTitle))
                });
            }

        }
    }
}