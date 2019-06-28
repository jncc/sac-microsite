using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Helpers.Generators;

namespace JNCC.Microsite.SAC.Generators
{
    public static class Generator
    {

        public static void MakeSite(GeneratorConfig config, string basePath = "", bool generateSearchDocuments = false, string searchIndex = null)
        {
            var serviceScopeFactory = ServiceScopeFactory.GetServiceScopeFactory();

            var sitemapEntires = new List<SitemapEntry>();

            Console.WriteLine("Generating Error Pages");
            ErrorPageGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex);

            Console.WriteLine("Generating Site Pages");
            SitesGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex, sitemapEntires);

            Console.WriteLine("Generating Habitat Pages");
            HabitatsGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex, sitemapEntires);

            Console.WriteLine("Generating Species Pages");
            SpeciesGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex, sitemapEntires);

            Console.WriteLine("Generating Misc Pages");
            MiscGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex, sitemapEntires);

            Console.WriteLine("Generating sitemap.xml and robots.txt");
            SitemapGenerator.Generate(basePath, sitemapEntires);
            RobotsGenerator.Generate(basePath);
        }
    }
}