using System;
using System.Collections.Generic;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class Search : Page
    {
        public List<(string EUCode, string Name)> Sites { get; set; }
    }
}