using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JNCC.Microsite.SAC.Helpers.Generators;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Helpers.Website;
using Microsoft.Extensions.DependencyInjection;

namespace JNCC.Microsite.SAC.Generators.PageBuilders
{
    public static class MiscPageBuilder
    {
        public static Task<string> RenderGibraltarPage(IServiceScopeFactory scopeFactory, GeneratorConfig config)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var helper = RenderHelper.GetRendererHelper(serviceScope);

                var model = new Page
                {
                    GeneratorConfig = config,
                    Breadcrumbs = new List<(string href, string text, bool current)> { ("/", "Home", true), ("/site/", "Site", true), ("/site/gibraltar", "Natura 2000 in Gibraltar", true) },
                    CurrentSection = "Site",
                    Title = StringHelpers.RemoveHTMLTags(Page.DefaultTitle),
                    MetaDescription = "European Directives, including the Habitats and Birds Directives, apply to the the UK overseas territory of Gibraltar. Although cSACs in Gibraltar are submitted to the European Commission via Defra in the same way as sites in the metropolitan UK, the Government of Gibraltar is responsible for overseeing the selection and management of sites within its territory. Copies of the Natura data forms, as submitted to the European Commission, can be downloaded from here.\n Los directorios europeos, incluyendo los habitat y los directorios de los pájaros, se aplican al territorio de ultramar UK de Gibraltar. Aunque los SACOS en Gibraltar se someten a la Comisión de las Comunidades Europeas vía Defra de la misma manera que sitios en el Reino Unido metropolitana, el gobierno de Gibraltar es responsable de supervisar la selección y la gerencia de sitios dentro de su territorio. La información adicional está disponible del gobierno de Gibraltar.",
                    MetaKeywords = new List<string>{
                        "Bay of Gibraltar", "cSAC", "candidate SACs", "Candidato", "Espana", "España", "Estrecho de Gibraltar", "Europe", "Gibraltar", "Gibralter", "Gibraltre", "GONHS", "Marine", "Mediterránea", "Mediterranea", "Mediterranean", "Mediteranean", "OT", "Overseas", "Territory", "Territories", "The Rock", "Rock of Gibraltar", "Southern Waters of Gibraltar", "Strait of Gibraltar", "Spain"
                    }
                };

                return helper.RenderViewToStringAsync("Views/Misc/Gibraltar.cshtml", model);
            }
        }

    }
}
