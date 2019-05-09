using JNCC.Microsite.SAC.Generators.Renderers;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace JNCC.Microsite.SAC.Helpers.Generators
{
    public static class RenderHelper
    {
        public static RazorViewToStringRenderer GetRendererHelper(IServiceScope serviceScope)
        {
            return serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
        }

        public static void WriteToFile(string path, string content)
        {
            // Create Directory if it does not already exist
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // Write content to the file
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(content);
            }
        }
    }
}