using System;
using System.Collections.Generic;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class Breadcrumb
    {
        public List<(string href, string text, bool current)> Breadcrumbs { get; set; }
    }
}