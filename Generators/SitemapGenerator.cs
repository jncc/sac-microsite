using System;
using System.Collections.Generic;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Helpers;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace JNCC.Microsite.SAC.Generators
{
    public static class SitemapGenerator
    {
        public static void Generate(string basePath, List<SitemapEntry> sitemapEntries)
        {
            XNamespace xmlNS = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var modifiedDateString = XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc);

            var sitemapContent = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement(xmlNS + "urlset",
                    from entry in sitemapEntries
                    select
                        new XElement(xmlNS + "url",
                        new XElement(xmlNS + "loc", entry.URL),
                        new XElement(xmlNS + "lastmod", String.IsNullOrEmpty(entry.DateModified) ? modifiedDateString : entry.DateModified),
                        new XElement(xmlNS + "changefreq", String.IsNullOrEmpty(entry.ChangeFreq) ? "monthly" : entry.ChangeFreq))
                )
            );

            FileHelper.WriteToFile(FileHelper.GetActualFilePath(basePath, "output/html/sitemap.xml"), sitemapContent.ToString());
        }
    }
}