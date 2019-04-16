using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Data;

namespace JNCC.Microsite.SAC.Website.Helpers
{
    public static class SpeciesInterestFeatureHelpers
    {
        public static List<InterestFeatureOccurrence> GetInterestFeatures(List<InterestFeatureOccurrence> features, bool primary)
        {
            if (primary) {
                return features.FindAll(f => f.GlobalGrade == "A" || f.GlobalGrade == "B");
            }
            return features.FindAll(f => f.GlobalGrade == "C");
        }
    }
}