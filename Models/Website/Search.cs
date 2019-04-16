using System;
using System.Collections.Generic;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class Search
    {
        public List<(string Code, string Name)> Sites { get; set; }
    }
}