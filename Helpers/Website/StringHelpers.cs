using System;
using System.Text.RegularExpressions;

namespace JNCC.Microsite.SAC.Helpers.Website
{
    public static class StringHelpers
    {
        public static string RemoveHTMLTags(string input, string replaceWith = "")
        {
            return Regex.Replace(input, "<[^>]*>", replaceWith);
        }     
    }
}