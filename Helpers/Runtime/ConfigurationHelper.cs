using System;
using System.IO;

namespace JNCC.Microsite.SAC.Helpers.Runtime
{
    public static class ConfigurationHelper
    {
        public static string GetDefaultWebRoot() 
        {
            return Path.Combine(Environment.CurrentDirectory, "docs");
        }
    }
}