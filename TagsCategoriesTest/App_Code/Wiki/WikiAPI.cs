using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Threading;
using System.Web.Services;
using TagsCategoriesTest.App_Code.Utils;

/// <summary>
/// Summary description for WikiAPI
/// </summary>
/// 
namespace TagsCategoriesTest.App_Code.Wiki
{
    public class WikiAPI
    {
        private static string _xmlFilePath;
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        #region Properties
        public static XDocument WikiXML
        {
            get
            {
                return XDocument.Load(XmlFilePath);
            }
        }

        public static string XmlFilePath
        {
            get
            {
                return _xmlFilePath;
            }
            private set
            {
                _xmlFilePath = HttpContext.Current.Server.MapPath(@"~/App_Data/Wiki.xml");
            }
        }
        #endregion Properties

        #region Methods
        public void CreateEntry(WikiDTO wiki)
        {
            if (wiki == null)
            {
                throw new ArgumentNullException(nameof(wiki));
            }
            // Lock 
            _readWriteLock.EnterWriteLock();

            try
            {
                // Add new wiki 
                AddWikiEntry(wiki);
            }
            finally
            {
                // Release lock
                _readWriteLock.ExitWriteLock();
            }
        }

        private void AddWikiNodes(HashSet<string> data, string xmlParent, string xmlChild, string xmlChildAttribute)
        {
            foreach (var item in data)
            {
                if (!XmlUtils.XmlElementExist(WikiXML, xmlParent, xmlChildAttribute, item))
                {
                    // Add it
                    WikiXML.Root
                            .Element(xmlParent)
                            .Add(
                                new XElement(xmlChild, new XAttribute("Id", XmlUtils.GenerateNodeId(WikiXML, xmlParent)),
                                new XAttribute(xmlChildAttribute, item))
                            );
                }
            }
        }

        // Creates WikiEntry
        private void AddWikiEntry(WikiDTO wiki)
        {
            // Add all new tags / categories (If they don't already exist in the XML)
            AddWikiNodes(wiki.Categories, "Categories", "Category", "Title");
            AddWikiNodes(wiki.Tags, "Tags", "Tag", "Title");

            // Add the WikiEntry
            XElement wikiEntry =
                    new XElement("WikiEntry",
                        new XAttribute("Id", Guid.NewGuid()),
                        new XAttribute("CreatedBy", "kaloyan@kukui.com"),
                        new XAttribute("CreatedAt", DateTimeOffset.UtcNow.ToString()),
                        new XAttribute("UpdatedBy", "kaloyan@kukui.com"),
                        new XAttribute("UpdatedAt", DateTimeOffset.UtcNow.ToString()),
                        new XAttribute("CategoryIds", XmlUtils.FilterHashData(WikiXML, wiki.Categories, "Categories", "Id")),
                        new XAttribute("TagIds", XmlUtils.FilterHashData(WikiXML, wiki.Tags, "Tags", "Id")),
                        new XElement("Title", wiki.Title),
                        new XElement("Content", new XCData(wiki.Content))
                    );
            WikiXML.Root.Element("WikiEntries").Add(wikiEntry);
            WikiXML.Save(XmlFilePath);
        }

        // Show entry by id
        public void ShowEntry(string category)
        {
            var data = from x in WikiXML.Root
                        .Elements("WikiEntries")
                        .Elements("WikiEntry")
                        where x.Attribute("CategoryIds").Value == category
                        select new
                        {
                            Title = x.Attribute("Title").Value,
                            Id = x.Attribute("Id").Value,
                        };
        }

        // Edit Wiki Entry
        public static void EditEntry(string wikiId, string wikiTitle, string wikiContent)
        {
            var data = WikiXML.Root
                                .Elements("WikiEntries")
                                .Elements()
                                .Where(p => p.Attribute("Id")
                                .Value == wikiId)
                                .FirstOrDefault();

            data.Element("Title").Value = wikiTitle;
            data.Element("Content").Value = wikiContent;
            data.Attribute("UpdatedAt").Value = DateTimeOffset.UtcNow.ToString();
            XmlUtils.SaveXML(WikiXML, XmlFilePath);
        }

        // Delete Wiki Entry
        [WebMethod]
        public static void DeleteEntry(string id)
        {
            WikiXML.Root
                    .Elements("WikiEntries")
                    .Elements()
                    .Where(p => p.Attribute("Id")
                    .Value == id)
                    .Remove();
        }
        #endregion Methods
    }
}