using System;
using System.Collections.Generic;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class Page
    {
        public List<(string href, string text, bool current)> Breadcrumbs { get; set; }
        public string CurrentSection { get; set; }
    }
}