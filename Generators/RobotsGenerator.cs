using JNCC.Microsite.SAC.Helpers;
using System.Text;

namespace JNCC.Microsite.SAC.Generators
{
    public static class RobotsGenerator
    {
        public static void Generate(string basePath)
        {
            var builder = new StringBuilder();
            builder.AppendLine("User-agent: *");
            builder.AppendLine("Dissallow: /frontend/");
            builder.AppendLine("Dissallow: /output/");
            builder.AppendLine("Dissallow: /images/");
            builder.AppendLine("Sitemap: https://sac.jncc.gov.uk/sitemap.xml");

            FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, "output/html/robots.txt"), builder.ToString());
        }
    }
}