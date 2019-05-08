using System;
using System.Text.RegularExpressions;

namespace JNCC.Microsite.SAC.Website.Helpers
{
    public static class StringHelpers
    {
        public static string RemoveHTMLTags(string input)
        {
            return Regex.Replace(input, "<[^>]*>", String.Empty);
        }
    }
}