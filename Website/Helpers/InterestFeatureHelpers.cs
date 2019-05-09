using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Data;

namespace JNCC.Microsite.SAC.Website.Helpers
{
    public static class InterestFeatureHelpers
    {
        public static string GetPlainIntegerCode(string code)
        {
            return code.Substring(1);
        }

        public static bool IsSpeciesCode(string code)
        {
            if (code.StartsWith("S"))
            {
                return true;
            }

            if (code.StartsWith("H"))
            {
                return false;
            }

            throw new ArgumentException(String.Format("{0} is not a valid code, expected habitat (Hxxxx) | species (Sxxxx)"));
        }

        public static bool IsHabitatCode(string code)
        {
            if (code.StartsWith("H"))
            {
                return true;
            }

            if (code.StartsWith("S"))
            {
                return false;
            }

            throw new ArgumentException(String.Format("{0} is not a valid code, expected habitat (Hxxxx) | species (Sxxxx)"));
        }

        public static string GetInterestFeaturePDFResourceURL(string code)
        {
            return String.Format("https://jncc.gov.uk/assets/REPORT-312/{0}.pdf", code);
        }        

        public static string GetFeatureUrl(string code)
        {
            if (IsHabitatCode(code))
            {
                return String.Format("/habitat/{0}", code);
            }
            else
            {
                return String.Format("/species/{0}", code);
            }
        }

        public static string GetAnnexString(string code)
        {
            if (IsHabitatCode(code))
            {
                return "Annex I habitat";
            }
            return "Annex II species";
        }

        public static string GetUKDistributionAltImageText(string code, string layTitle, string Name)
        {
            string template = "UK Distribution of {0} {1} {2} {3}. Click image to view detailed UK distribution and {4}.";

            if (IsHabitatCode(code))
            {
                return String.Format(template, GetAnnexString(code), GetPlainIntegerCode(code), layTitle, Name, "extent information for this habitat");
            }

            return String.Format(template, GetAnnexString(code), GetPlainIntegerCode(code), layTitle, Name, "population size information for this species");
        }

        public static string GetSACDistributionAltImageText(string code, string layTitle, string Name)
        {
            string template = "Distribution of SACs with {0} {1} {2} ({3}). Click image to view detailed information on SACs selected for this {4}.";

            if (IsHabitatCode(code))
            {
                return String.Format(template, GetAnnexString(code), GetPlainIntegerCode(code), layTitle, Name, "habitat");
            }

            return String.Format(template, GetAnnexString(code), GetPlainIntegerCode(code), layTitle, Name, "species");
        }

        public static string GetCompareDistributionURL(string code)
        {
            if (IsHabitatCode(code))
            {
                return String.Format("/habitat/{0}/comparison", code);
            }
            return String.Format("/species/{0}/comparison", code);
        }

        public static string GetMapURL(string code)
        {
            if (IsHabitatCode(code))
            {
                return String.Format("/habitat/{0}/map", code);
            }
            return String.Format("/species/{0}/map", code);
        }

        public static List<InterestFeatureOccurrence> GetInterestFeatures(List<InterestFeatureOccurrence> features, bool primary)
        {
            if (primary)
            {
                return features.FindAll(f => f.GlobalGrade == "A" || f.GlobalGrade == "B");
            }
            return features.FindAll(f => f.GlobalGrade == "C");
        }
    }
}