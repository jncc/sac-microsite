using System;
using System.Collections.Generic;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class Page
    {
        public List<(string href, string text, bool display)> Breadcrumbs { get; set; }
        public string CurrentSection { get; set; }
        public bool DisplayBreadcrumb { get; set; } = true;
        public bool HeroImage { get; set; } = false;
    }
}