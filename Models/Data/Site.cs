using System;
using System.Collections.Generic;

namespace JNCC.Microsite.SAC.Models.Data
{
    public class Site
    {
        public string EUCode { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string CountryFull { get; set; }
        public double Area { get; set; }
        public string GridReference { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? XCoord { get; set; }
        public int? YCoord { get; set; }
        public string LocalAuthority { get; set; }
        public string LinkText { get; set; }
        public int StatusCode { get; set; }
        public string StatusShort { get; set; }
        public string StatusLong { get; set; }
        public List<SiteFeature> Features { get; set; }
        public List<SiteCharacter> Character { get; set; }
    }

    public class SiteFeature
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string InterestStatus { get; set; }
        public string InterestStatusLong { get; set; }
        public string GlobalGrade { get; set; }
        public string PrimaryText { get; set; }
        public string SecondaryText { get; set; }
        public string LocalAuthority { get; set; }
        public string LayTitle { get; set; }
        public bool Priority { get; set; }
    }

    public class SiteCharacter
    {
        public string Character { get; set; }
        public double Coverage { get; set; }
    }
}