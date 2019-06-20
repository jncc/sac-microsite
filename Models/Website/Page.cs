using System.Linq;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Generators;
using JNCC.Microsite.SAC.Helpers.Website;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class Page
    {
        public static List<string> DEFAULT_METADATA_KEYWORDS = new List<string> {
            "Habitats Directive", "Natura 2000", "Protected Aeas", "Site Designation", "Site Selection", "Special Area of Conservation", "SAC", "United Kingdom", "UK"
        };

        private string _metadataDescription;
        private IEnumerable<string> _metadataKeywords;

        public List<(string href, string text, bool display)> Breadcrumbs { get; set; }
        public string CurrentSection { get; set; }
        public bool DisplayBreadcrumb { get; set; } = true;
        public bool HeroImage { get; set; } = false;
        public string MetaDescription
        {
            get
            {
                return this._metadataDescription;
            }
            set
            {
                this._metadataDescription = StringHelpers.RemoveHTMLTags(value);
            }
        }
        public IEnumerable<string> MetaKeywords
        {
            get
            {
                return this._metadataKeywords;
            }
            set
            {
                // Remove HTML tags and commas from the input strings and add the default metadata keywords
                this._metadataKeywords = value
                    .Select(v => StringHelpers.FixHTMLStringForMetaKeywords(v))
                    .Concat(DEFAULT_METADATA_KEYWORDS);
            }
        }
        public string Title { get; set; }
        public static string DefaultTitle = "Special Areas of Conservation";
        public GeneratorConfig GeneratorConfig { get; set; }
    }
}