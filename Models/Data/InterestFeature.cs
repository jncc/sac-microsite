using System;
using System.Collections.Generic;

namespace JNCC.Microsite.SAC.Models.Data
{
    public class InterestFeature
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string LayTitle { get; set; }
        public double SectionNumber { get; set; }
        public string SectionTitle { get; set; }
        public string InterestGroup { get; set; }
        public string FeatureDescription { get; set; }
        public string EUStatus { get; set; }
        public string UKStatus { get; set; }
        public string Rationale { get; set; }
        public bool Priority { get; set; }
        public int Total { get; set; }
        public List<InterestFeatureOccurrence> Occurrences { get; set; }
    }

    public class InterestFeatureOccurrence
    {
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string InterestStatus { get; set; }
        public string InterestStatusLong { get; set; }
        public string GlobalGrade { get; set; }
        public string PrimaryText { get; set; }
        public string SecondaryText { get; set; }
        public string LocalAuthority { get; set; }
        public bool IsHabitat { get; set; }
        public bool IsSpecies { get; set; }
    }
}