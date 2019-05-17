using System;
using System.Collections.Generic;

namespace JNCC.Microsite.SAC.Models.Search
{
    public class SearchDocument
    {
        public string id { get; set; }
        public string site { get; set; } = "sac";
        public string title { get; set; }
        public string content { get; set; }
        public string data_type { get; set; }
        public string url { get; set; }
        public List<Keyword> keywords { get; set; }
    }
}
