using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Helpers.Website;

namespace JNCC.Microsite.SAC.Models.Data
{
    public class InterestFeature
    {
        private string _featureDescription;
        private string _euStatus;
        private string _ukStatus;
        private string _rationale;

        public string Code { get; set; }
        public string Name { get; set; }
        public string LayTitle { get; set; }
        public double SectionNumber { get; set; }
        public string SectionTitle { get; set; }
        public string InterestGroup { get; set; }
        public string FeatureDescription
        {
            get
            {
                return _featureDescription;
            }
            set
            {
                this._featureDescription = StringHelpers.FixHTMLString(value);
            }
        }
        public string EUStatus
        {
            get
            {
                return _euStatus;
            }
            set
            {
                this._euStatus = StringHelpers.FixHTMLString(value);
            }
        }
        public string UKStatus
        {
            get
            {
                return _ukStatus;
            }
            set
            {
                this._ukStatus = StringHelpers.FixHTMLString(value);
            }
        }
        public string Rationale
        {
            get
            {
                return _rationale;
            }
            set
            {
                this._rationale = StringHelpers.FixHTMLString(value);
            }
        }
        public bool Priority { get; set; }
        public int Total { get; set; }
        public InterestFeatureMapData MapData { get; set; }
        public List<InterestFeatureOccurrence> Occurrences { get; set; }
    }

    public class InterestFeatureOccurrence
    {
        private string _interestStatus;
        private string _interestStatusLong;
        private string _primaryText;
        private string _secondayText;

        public string SiteCode { get; set; }
        public string SiteName { get; set; }
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
                return _secondayText;
            }
            set
            {
                this._secondayText = StringHelpers.FixHTMLString(value);
            }
        }
        public string LocalAuthority { get; set; }
        public bool IsHabitat { get; set; }
        public bool IsSpecies { get; set; }
    }

    public class InterestFeatureMapData
    {
        public string MapSources { get; set; }
        public string MapExplanation { get; set; }
        public string Units { get; set; }
        public string England { get; set; }
        public string Scotland { get; set; }
        public string Wales { get; set; }
        public string NorthernIreland { get; set; }
        public string UKOffshoreWaters { get; set; }
        public string TotalUkPopulation { get; set; }
    }
}