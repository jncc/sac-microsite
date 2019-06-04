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
using JNCC.Microsite.SAC.Helpers.Generators;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;

namespace JNCC.Microsite.SAC.Generators
{
    public static class Generator
    {

        public static void MakeSite(GeneratorConfig config, string basePath = "", bool generateSearchDocuments = false, string searchIndex = null)
        {
            var serviceScopeFactory = ServiceScopeFactory.GetServiceScopeFactory();

            Console.WriteLine("Generating Error Pages");
            ErrorPageGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex);

            Console.WriteLine("Generating Site Pages");
            SitesGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex);

            Console.WriteLine("Generating Habitat Pages");
            HabitatsGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex);

            Console.WriteLine("Generating Species Pages");
            SpeciesGenerator.Generate(serviceScopeFactory, config, basePath, generateSearchDocuments, searchIndex);
        }
    }
}