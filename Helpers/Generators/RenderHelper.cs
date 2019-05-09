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
    }
}