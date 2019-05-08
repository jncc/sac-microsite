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
    public static class SearchPageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, IEnumerable<(string EUCode, string Name)> sites)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                var model = new Search
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), },
                    CurrentSection = "Search",
                    Sites = sites.ToList(),
                    Title = StringHelpers.RemoveHTMLTags(Page.DefaultTitle)
                };

                return helper.RenderViewToStringAsync("Views/Search.cshtml", model);
            }
        }

    }
}
