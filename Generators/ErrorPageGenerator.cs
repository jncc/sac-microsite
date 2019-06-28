using System;
using Microsoft.Extensions.DependencyInjection;
using JNCC.Microsite.SAC.Generators.PageBuilders;
using JNCC.Microsite.SAC.Helpers;

namespace JNCC.Microsite.SAC.Generators
{
    public static class ErrorPageGenerator
    {
        public static void Generate(IServiceScopeFactory serviceScopeFactory, GeneratorConfig config, string basePath, bool generateSearchDocuments, string searchIndex)
        {
            Console.WriteLine("Generate 404 Page");
            var notFoundContent = ErrorPageBuilder.RenderPage(serviceScopeFactory, 404, config).Result;
            FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, "output/html/404.html"), notFoundContent);
        }
    }
}
