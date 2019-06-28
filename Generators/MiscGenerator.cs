using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using JNCC.Microsite.SAC.Generators.PageBuilders;
using JNCC.Microsite.SAC.Helpers;

namespace JNCC.Microsite.SAC.Generators
{
    public static class MiscGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory, GeneratorConfig config, string basePath, bool generateSearchDocuments, string searchIndex, List<SitemapEntry> sitemapEntries)
        {
            var gibraltarPageContent = MiscPageBuilder.RenderGibraltarPage(serviceScopeFactory, config).Result;
            FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, "output/html/site/gibraltar.html"), gibraltarPageContent);
            sitemapEntries.Add(new SitemapEntry
            {
                URL = "/site/gibraltar"
            });

            if (generateSearchDocuments)
            {
                FileHelper.WriteJSONToFile(
                    FileHelper.GetActualFilePath(basePath, "output/search/site/gibraltar.json"),
                    SearchHelpers.GetMiscPageSearchDocument(searchIndex, "MISC-GIBRALTAR", "Natura 2000 in Gibraltar", gibraltarPageContent, "/site/gibraltar")
                );
            }
        }
    }
}