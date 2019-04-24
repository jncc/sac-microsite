using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Renderers;
using Microsoft.Extensions.DependencyInjection;


namespace JNCC.Microsite.SAC.Website
{
    public static class PageBuilders
    {
        public static Task<string> RenderSearchPage(IServiceScopeFactory scopeFactory, IEnumerable<(string EUCode, string Name)> sites)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = GetRendererHelper(serviceScope);

                var model = new Search
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/search", "Search", true) },
                    CurrentSection = "Search",
                    Sites = sites.ToList()
                };

                return helper.RenderViewToStringAsync("Views/Search.cshtml", model);
            }
        }

        public static Task<string> RenderSitePage(IServiceScopeFactory scopeFactory, Site site)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = GetRendererHelper(serviceScope);

                var model = new SitePage
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/site", "Site", false), (String.Format("/site/{0}", site.EUCode), site.Name, true) },
                    CurrentSection = "Site",
                    Site = site
                };

                return helper.RenderViewToStringAsync("Views/Site.cshtml", model);
            }
        }

        public static Task<string> RenderHabitatPage(IServiceScopeFactory scopeFactory, InterestFeature feature)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = GetRendererHelper(serviceScope);

                var model = new InterestFeaturePage
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/habitat", "Habitat", false), (string.Format("/habitat/{0}", feature.Code), feature.Name, true) },
                    CurrentSection = "Habitat",
                    InterestFeature = feature
                };

                return helper.RenderViewToStringAsync("Views/HabitatInterestFeature.cshtml", model);
            }
        }

        public static Task<string> RenderSpeciesPage(IServiceScopeFactory scopeFactory, InterestFeature feature)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = GetRendererHelper(serviceScope);

                var model = new InterestFeaturePage
                {
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/species", "Species", false), (string.Format("/species/{0}", feature.Code), feature.Name, true) },
                    CurrentSection = "Species",
                    InterestFeature = feature
                };

                return helper.RenderViewToStringAsync("Views/SpeciesInterestFeature.cshtml", model);
            }
        }

        public static Task<string> RenderErrorPage(IServiceScopeFactory scopeFactory, int error)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = GetRendererHelper(serviceScope);

                return helper.RenderViewToStringAsync("Views/Error/404.cshtml", new Page{
                    Breadcrumbs = new List<(string href, string text, bool current)>{ ("/", "Home", false), ("/404.html", "Page Not Found", true)},
                    CurrentSection = null
                });
            }
        }

        private static RazorViewToStringRenderer GetRendererHelper(IServiceScope serviceScope)
        {
            return serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
        }
    }
}