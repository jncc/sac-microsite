using System;

namespace JNCC.Microsite.SAC.Models.Website
{
    public class SitemapEntry
    {
        public static string DEFAULT_BASE_URL_SCHEME = "https";
        public static string DEFAULT_BASE_URL = "sac.jncc.gov.uk";

        private string _url;

        public string BaseURLScheme { get; set; } = null;
        public string BaseURL { get; set; } = null;
        public string URL
        {
            get
            {
                return _url;
            }
            set
            {
                var uriBuilder = new UriBuilder();

                uriBuilder.Scheme = String.IsNullOrEmpty(this.BaseURLScheme) ? DEFAULT_BASE_URL_SCHEME : this.BaseURLScheme;
                uriBuilder.Host = String.IsNullOrEmpty(this.BaseURLScheme) ? DEFAULT_BASE_URL : this.BaseURLScheme;
                uriBuilder.Path = value;

                this._url = uriBuilder.ToString();
            }
        }
        public string DateModified { get; set; }
        public string ChangeFreq { get; set; } = "monthly";
    }
}