using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Search;

namespace JNCC.Microsite.SAC.Helpers
{
    public static class SearchHelpers
    {
        public static SearchDocumentWrapper GetHabitatPageSearchDocument(string index, string code, string name, string content)
        {
            return new SearchDocumentWrapper
            {
                index = index,
                document = SearchHelpers.GetDocument(code, name, content,
                    new List<Keyword> {
                        new Keyword {
                            vocab = "https://vocab.jncc.gov.uk/sac",
                            value = "habitat"
                        }
                    }
                )
            };
        }

        public static SearchDocumentWrapper GetSpeciesPageSearchDocument(string index, string code, string name, string content)
        {
            return new SearchDocumentWrapper
            {
                index = index,
                document = SearchHelpers.GetDocument(code, name, content,
                    new List<Keyword> {
                        new Keyword {
                            vocab = "https://vocab.jncc.gov.uk/sac",
                            value = "species"
                        }
                    }
                )
            };
        }

        public static SearchDocumentWrapper GetSitePageSearchDocument(string index, string code, string name, string content)
        {
            return new SearchDocumentWrapper
            {
                index = index,
                document = SearchHelpers.GetDocument(code, name, content,
                    new List<Keyword> {
                        new Keyword {
                            vocab = "https://vocab.jncc.gov.uk/sac",
                            value = "site"
                        }
                    }
                )
            };
        }

        public static string GetDocumentURLFromCode(string code)
        {
            if (code.StartsWith("H"))
            {
                return String.Format("https://sac.jncc.gov.uk/habitat/{0}", code);
            }
            else if (code.StartsWith("S"))
            {
                return String.Format("https://sac.jncc.gov.uk/species/{0}", code);
            }
            else if (code.StartsWith("UK"))
            {
                return String.Format("https://sac.jncc.gov.uk/site/{0}", code);
            }
            throw new ArgumentException(String.Format("Expected Code to be of the form [H*|S*|UK*] but it was {0}", code));
        }

        public static SearchDocument GetDocument(string code, string name, string content, List<Keyword> keywords)
        {
            return new SearchDocument
            {
                id = String.Format("SAC-MICROSITE-{0}", code),
                title = name,
                content = content,
                data_type = "publication",
                url = SearchHelpers.GetDocumentURLFromCode(code),
                keywords = keywords
            };
        }
    }
}