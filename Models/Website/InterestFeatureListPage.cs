using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Data;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class InterestFeatureListPage : Page
    {
        public string Type { get; set; }
        public List<InterestFeatureSection> InterestFeatureSections { get; set; }
    }

    public class InterestFeatureSection
    {
        public string SectionTitle { get; set; }
        public List<InterestFeature> InterestFeatures { get; set; }
    }
}