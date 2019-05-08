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
    public static class ErrorPageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, int error)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                return helper.RenderViewToStringAsync("Views/Error/404.cshtml", new Page
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/404.html", "Page Not Found", true) },
                    CurrentSection = null,
                    DisplayBreadcrumb = false,
                    Title = String.Format("Page not found - {0}", Page.DefaultTitle)
                });
            }
        }
    }
}
