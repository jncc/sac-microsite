using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using JNCC.Microsite.SAC.Website.Helpers;

namespace JNCC.Microsite.SAC.Generators.Helpers
{
    public static class SearchHelper
    {
        public static string GenerateSearchText(string siteHtml)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(siteHtml);

            return Regex.Replace(
                HtmlEntity.DeEntitize(
                    StringHelpers.RemoveHTMLTags(
                        htmlDoc.DocumentNode.SelectSingleNode("//html/body/div/main").InnerHtml,
                        " "
                    )
                ),
                "\\s\\s+",
                "\n"
            );
        }
    }
}