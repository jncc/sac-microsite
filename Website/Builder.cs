using System;
using System.Collections.Generic;
using System.IO;
using RazorLight;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Models.Data;
using Newtonsoft.Json;
using System.Linq;
using System.Dynamic;
using System.Threading.Tasks;


namespace JNCC.Microsite.SAC.Website 
{
    public class Builder
    {
        public async void BuildSearch() {
            var engine = new RazorLightEngineBuilder()
                            .UseEmbeddedResourcesProject(typeof(Builder))
                            .UseMemoryCachingProvider()
                            .Build();

            using (StreamReader r = new StreamReader("output/json/sites.json"))
            {
                string json = r.ReadToEnd();
                List<Site> sites = JsonConvert.DeserializeObject<List<Site>>(json);

                var m = new JNCC.Microsite.SAC.Models.Website.Search
                {
                    Sites = sites.Select(s => (s.EUCode, s.Name)).ToList()
                };

                dynamic viewBag = new ExpandoObject();
                viewBag.CurrentSection = "Search";
                viewBag.Breadcrumbs = new List<(string, string, bool)> { ("/Search", "Search", true) };

                var rendered = await engine.CompileRenderAsync("Search", m, viewBag);
                var x = rendered;
                // return rendered;
            }
        }          
    }
}
