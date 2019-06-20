using System;
using System.Text.RegularExpressions;

namespace JNCC.Microsite.SAC.Helpers.Website
{
    public static class StringHelpers
    {
        public static string FixHTMLStringForMetaKeywords(string input) {
            // Remove HTML tags and , characters from the input string
            input = RemoveHTMLTags(input.Replace(",", ""));
            return input;
        }

        public static string RemoveHTMLTags(string input, string replaceWith = "")
        {
            return Regex.Replace(input, "<[^>]*>", replaceWith);
        }

        public static string FixHTMLString(string input) {
            return StringHelpers.FixAnchorTags(input);
        }

        public static string FixAnchorTags(string input)
        {
            input = Regex.Replace(input, "<[Aa] href=&quot;", "<a targe=\"_blank\" href=\"");
            input = Regex.Replace(input, "&quot;>", "\">");
            input = Regex.Replace(input, "</A>", "</a>");
            return input;
        }
    }
}