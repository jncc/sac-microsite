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
    public static class SiteListPageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, GeneratorConfig config, string header, string subject, (string href, string text, bool display)? breadcrumb, List<RegionalSites> sites)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);
                
                return helper.RenderViewToStringAsync("Views/SiteList.cshtml", new SiteList
                {
                    GeneratorConfig = config,
                    DisplayBreadcrumb = false,                    
                    CurrentSection = "Site",
                    HeaderText = header,
                    SubjectHTML = subject,
                    RegionalSites = sites,
                    Title = StringHelpers.RemoveHTMLTags(String.Format("List of {0} - {1}", header, Page.DefaultTitle)), 
                    MetaDescription = "Selection of Special Areas of Conservation in the UK, second edition, JNCC (2002)",
                    MetaKeywords = new List<string>{}
                });
            }

        }
    }
}