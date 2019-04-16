using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Data;

namespace JNCC.Microsite.SAC.Website.Helpers
{
    public static class SiteHelpers
    {
        public static string GetSiteURLFromCode(string code)
        {
            return String.Format("/site/{0}", code);
        }
        public static string GetCoverageString(double? coverage)
        {
            return (coverage != null && coverage != double.NaN) ? coverage.ToString() : "Unknown";
        }

        public static string IsPriorityFeature(bool priority)
        {
            if (priority)
            {
                return "&nbsp;<span style=\"font-size:small;\"> * Priority feature</span>";
            }
            return "";
        }

        public static List<SiteFeature> GetAnnexInterestFeature(List<SiteFeature> features, bool species = true, bool primary = true)
        {
            if (species)
            {
                if (primary)
                {
                    return features
                        .FindAll(f => f.Code.StartsWith("S"))
                        .FindAll(f => f.GlobalGrade == "A" || f.GlobalGrade == "B");
                }

                return features
                    .FindAll(f => f.Code.StartsWith("S"))
                    .FindAll(f => f.GlobalGrade == "C");
            }
            else
            {
                if (primary)
                {
                    return features
                        .FindAll(f => f.Code.StartsWith("H"))
                        .FindAll(f => f.GlobalGrade == "A" || f.GlobalGrade == "B");
                }

                return features
                    .FindAll(f => f.Code.StartsWith("H"))
                    .FindAll(f => f.GlobalGrade == "C");
            }
        }
    }
}
