using System;

namespace JNCC.Microsite.SAC.Models.Search
{
    public class SearchDocumentWrapper
    {
        public string verb { get; set; } = "upsert";
        public string index { get; set; }
        public SearchDocument document { get; set; }
    }
}
