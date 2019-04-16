using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Data;

namespace JNCC.Microsite.SAC.Website.Helpers
{
    public static class InformationFeatureHelpers
    {
        public static string GetPlainIntegerCode(string code)
        {
            return code.Substring(1);
        }

        public static string GetFeatureUrl(string code)
        {
            if (code.StartsWith("H"))
            {
                return String.Format("/habitat/{1}", code);
            } else if (code.StartsWith("S")) {
                return String.Format("/species/{1}", code);
            }

            throw new ArgumentException(String.Format("{0} is not a valid code, expected habitat (Hxxxx) | species (Sxxxx)"));
        }

        public static string GetAnnexHtml(List<SiteFeature> features)
        {
            if (features.Count == 0)
            {
                return "<p>Not Applicable</p>";
            }

            var output = "";

            foreach (var feature in features)
            {
                output = string.Format("{0}<p><b>{1}</b><a href=\"{2}\">{3}</a></p>\n",
                    output,
                    GetPlainIntegerCode(feature.Code),
                    GetFeatureUrl(feature.Code),
                    feature.Name);
                
                if (!String.IsNullOrWhiteSpace(feature.PrimaryText)) {
                    output = String.Format("{0}<p>{1}</p>\n", output, feature.PrimaryText);
                }
                if (!String.IsNullOrWhiteSpace(feature.SecondaryText)) {
                    output = String.Format("{0}<p>{1}</p>\n", output, feature.SecondaryText);
                }
            }

            return output;
        }
    }
}