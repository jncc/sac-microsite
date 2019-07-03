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
    public static class SearchPageBuilder
    {
        public static Task<string> RenderPage(IServiceScopeFactory scopeFactory, GeneratorConfig config, IEnumerable<(string EUCode, string Name)> sites)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                var model = new Search
                {
                    GeneratorConfig = config,
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), },
                    HeroImage = true,
                    DisplayBreadcrumb = false,
                    CurrentSection = "Search",
                    Sites = sites.ToList(),
                    Title = StringHelpers.RemoveHTMLTags(Page.DefaultTitle), 
                    MetaDescription = "Selection of Special Areas of Conservation in the UK, second edition, JNCC (2002)",
                    MetaKeywords = new List<string>{}
                };

                return helper.RenderViewToStringAsync("Views/Search.cshtml", model);
            }
        }

    }
}
