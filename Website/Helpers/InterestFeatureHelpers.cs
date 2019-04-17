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

        public static string GetUkResourceURL(string code, bool habitat) {
            return String.Format("/{0}/{1}/distribution/uk", habitat ? "habitat": "species", code);
        }

        public static string GetCompareUKDistributionURL(string code, bool habitat) {
            return String.Format("/{0}/{1}/distribution/compare", habitat ? "habitat" : "species", code);
        }
        

        public static List<InterestFeatureOccurrence> GetInterestFeatures(List<InterestFeatureOccurrence> features, bool primary)
        {
            if (primary) {
                return features.FindAll(f => f.GlobalGrade == "A" || f.GlobalGrade == "B");
            }
            return features.FindAll(f => f.GlobalGrade == "C");
        }        
    }
}