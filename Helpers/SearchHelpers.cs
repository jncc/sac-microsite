using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using JNCC.Microsite.SAC.Models.Search;
using JNCC.Microsite.SAC.Helpers.Website;

namespace JNCC.Microsite.SAC.Helpers
{
    public static class SearchHelpers
    {
        public static string GenerateSearchText(string siteHtml)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(siteHtml);

            return Regex.Replace(
                HtmlEntity.DeEntitize(
                    StringHelpers.RemoveHTMLTags(
                        htmlDoc.DocumentNode.SelectSingleNode("//html/body/div/main").InnerHtml,
                        " "
                    )
                ),
                "\\s\\s+",
                " "
            );
        }

        public static SearchDocumentWrapper GetHabitatPageSearchDocument(string index, string code, string name, string content)
        {
            return new SearchDocumentWrapper
            {
                index = index,
                document = SearchHelpers.GetDocument(code, name, SearchHelpers.GenerateSearchText(content),
                    new List<Keyword> {
                        new Keyword {
                            vocab = "https://vocab.jncc.gov.uk/special-areas-of-conservation",
                            value = "Habitat"
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
                document = SearchHelpers.GetDocument(code, name, SearchHelpers.GenerateSearchText(content),
                    new List<Keyword> {
                        new Keyword {
                            vocab = "https://vocab.jncc.gov.uk/special-areas-of-conservation",
                            value = "Species"
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
                document = SearchHelpers.GetDocument(code, name, SearchHelpers.GenerateSearchText(content),
                    new List<Keyword> {
                        new Keyword {
                            vocab = "https://vocab.jncc.gov.uk/special-areas-of-conservation",
                            value = "Site"
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
                id = String.Format("SAC-MICROSITE-{0}", code).ToLower(),
                title = StringHelpers.RemoveHTMLTags(name),
                content = content,
                data_type = "publication",
                url = SearchHelpers.GetDocumentURLFromCode(code),
                keywords = keywords.Concat(new List<Keyword> {
                    new Keyword {
                        vocab = "http://vocab.jncc.gov.uk/protected-areas",
                        value = "Protected Areas"
                    },
                    new Keyword {
                        vocab = "http://vocab.jncc.gov.uk/jncc-publication-category",
                        value = "Protected sites monitoring"
                    }
                }).ToList()
            };
        }
    }
}