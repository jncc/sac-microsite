using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Helpers.Website;

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
        private string _interestStatus;
        private string _interestStatusLong;
        private string _primaryText;
        private string _secondaryText;

        public string Code { get; set; }
        public string Name { get; set; }
        public string InterestStatus
        {
            get
            {
                return _interestStatus;
            }
            set
            {
                this._interestStatus = StringHelpers.FixHTMLString(value);
            }
        }
        public string InterestStatusLong
        {
            get
            {
                return _interestStatusLong;
            }
            set
            {
                this._interestStatusLong = StringHelpers.FixHTMLString(value);
            }
        }
        public string GlobalGrade { get; set; }
        public string PrimaryText
        {
            get
            {
                return _primaryText;
            }
            set
            {
                this._primaryText = StringHelpers.FixHTMLString(value);
            }
        }
        public string SecondaryText
        {
            get
            {
                return _secondaryText;
            }
            set
            {
                this._secondaryText = StringHelpers.FixHTMLString(value);
            }
        }
        public string LocalAuthority { get; set; }
        public string LayTitle { get; set; }
        public bool Priority { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class SiteCharacter
    {
        public string Character { get; set; }
        public double Coverage { get; set; }
    }
}