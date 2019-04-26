using JNCC.Microsite.SAC.Generators.Renderers;
using Microsoft.Extensions.DependencyInjection;

namespace JNCC.Microsite.SAC.Generators.Helpers
{
    public static class RenderHelper
    {
        public static RazorViewToStringRenderer GetRendererHelper(IServiceScope serviceScope)
        {
            return serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
        }
    }
}