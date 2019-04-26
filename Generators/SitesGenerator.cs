using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using JNCC.Microsite.SAC.Generators.PageBuilders;

namespace JNCC.Microsite.SAC.Generators
{
    public static class SitesGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory)
        {
            using (StreamReader fileReader = new StreamReader("output/json/sites.json"))
            {
                List<Site> sites = JsonConvert.DeserializeObject<List<Site>>(fileReader.ReadToEnd());

                var searchPageContent = SearchPageBuilder.RenderPage(serviceScopeFactory, sites.Select(s => (s.EUCode, s.Name))).Result;

                using (StreamWriter writer = new StreamWriter("output/html/index.html"))
                {
                    writer.Write(searchPageContent);
                }

                foreach (var site in sites)
                {
                    var sitePageContent = SitePageBuilder.RenderPage(serviceScopeFactory, site).Result;

                    using (StreamWriter writer = new StreamWriter(String.Format("output/html/site/{0}.html", site.EUCode)))
                    {
                        writer.Write(sitePageContent);
                    }
                }

                // Regional Site Lists
                using (StreamWriter writer = new StreamWriter("output/html/site/index.html"))
                {
                    var siteListPageContent = SiteListPageBuilder.RenderPage(
                        serviceScopeFactory,
                        "Sites in the United Kingdom",
                        String.Format("<p>There are {0} designated SACs, SCIs or cSACs in the <b>United Kingdom</b> including cross border sites (excluding <a href=\"#\">Gibraltar</a>). Cross border sites are listed under both countries. Sites are sorted alphabetically within country.</p>", sites.Count),
                        null,
                        new List<RegionalSites>{
                        new RegionalSites {
                            Region = "England",
                            Sites = sites.Where(s => s.Country.Contains("E"))
                        },
                        new RegionalSites {
                            Region = "Northern Ireland",
                            Sites = sites.Where(s => s.Country.Contains("NI"))
                        },
                        new RegionalSites {
                            Region = "Scotland",
                            Sites = sites.Where(s => s.Country.Contains("S"))
                        },
                        new RegionalSites {
                            Region = "Wales",
                            Sites = sites.Where(s => s.Country.Contains("W"))
                        },
                        new RegionalSites {
                            Region = "Offshore",
                            Sites = sites.Where(s => s.Country.Contains("O"))
                        },
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/england.html"))
                {
                    var siteListPageContent = SiteListPageBuilder.RenderPage(
                        serviceScopeFactory,
                        "SACs in England",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>England</b> including cross border sites. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("E")).Count()),
                        ("/england", "England", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "England",
                                Sites = sites.Where(s => s.Country.Contains("E"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/northern-ireland.html"))
                {
                    var siteListPageContent = SiteListPageBuilder.RenderPage(
                        serviceScopeFactory,
                        "SACs in Northern Ireland",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>Northern Ireland</b> including cross border sites. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("NI")).Count()),
                        ("/northern-ireland", "Northern Ireland", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "Northern Ireland",
                                Sites = sites.Where(s => s.Country.Contains("NI"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/scotland.html"))
                {
                    var siteListPageContent = SiteListPageBuilder.RenderPage(
                        serviceScopeFactory,
                        "SACs in Scotland",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>Scotland</b> including cross border sites. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("S")).Count()),
                        ("/scotland", "Scotland", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "Scotland",
                                Sites = sites.Where(s => s.Country.Contains("S"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/wales.html"))
                {
                    var siteListPageContent = SiteListPageBuilder.RenderPage(
                        serviceScopeFactory,
                        "SACs in Wales",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>Wales</b> including cross border sites. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("S")).Count()),
                        ("/wales", "Wales", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "Wales",
                                Sites = sites.Where(s => s.Country.Contains("W"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

                using (StreamWriter writer = new StreamWriter("output/html/site/offshore.html"))
                {
                    var siteListPageContent = SiteListPageBuilder.RenderPage(
                        serviceScopeFactory,
                        "SACs in UK offshore waters",
                        String.Format("<p>There are {0} SACs, SCIs or cSACs in <b>UK offshore waters including those that cross the 12 mile limit</b>. Sites are sorted alphabetically.</p>",
                            sites.Where(s => s.Country.Contains("O")).Count()),
                        ("/offshore", "Offshore", true),
                        new List<RegionalSites>
                        {
                            new RegionalSites {
                                Region = "Offshore",
                                Sites = sites.Where(s => s.Country.Contains("O"))
                            }
                        }).Result;
                    writer.Write(siteListPageContent);
                }

            }

        }
    }
}