using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Data;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class SiteList : Page
    {
        public string HeaderText { get; set; }
        public string SubjectHTML { get; set; }
        public List<RegionalSites> RegionalSites { get; set; }
    }

    public class RegionalSites
    {
        public string Region { get; set; }
        public IEnumerable<Site> Sites { get; set; }
    }
}