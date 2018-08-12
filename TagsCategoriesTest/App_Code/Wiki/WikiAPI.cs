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
        #region Fields
        private static readonly string _xmlFilePath = HttpContext.Current.Server.MapPath(@"~/App_Data/Wiki.xml");
        private static readonly XDocument _wikiXml = XDocument.Load(_xmlFilePath);
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        #endregion Fields

        #region Properties
        public static XDocument WikiXML
        {
            get
            {
                return _wikiXml;
            }
        }

        public static string XmlFilePath
        {
            get
            {
                return _xmlFilePath;
            }
        }
        #endregion Properties

        #region Methods

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
                                new XElement(xmlChild,
                                new XAttribute("Id", XmlUtils.GenerateNodeId(WikiXML, xmlParent, xmlChild, xmlChildAttribute, item)),
                                new XAttribute(xmlChildAttribute, item),
                                new XAttribute("Referenced", XmlUtils.UpdateReferences(WikiXML, xmlParent, xmlChild)))
                            );
                }
                else
                {
                    var node = WikiXML.Descendants(xmlParent).Elements(xmlChild).Where(p => p.Attribute("Title").Value == item).FirstOrDefault();

                    if (node != null)
                    {
                        node.Attribute("Referenced").Value = (Convert.ToInt32(node.Attribute("Referenced").Value) + 1).ToString();
                    }

                }
            }
        }

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
            WikiXML.Save(_xmlFilePath);
        }


        // CRUD operations

        // Create wiki
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

        // Show wiki by id
        public static XElement GetWikiById(string parentNode, string expectedValue)
        {
            return XmlUtils.GetNodeByAttributeValue(WikiXML, parentNode, "Id", expectedValue);
        }

        // Edit wiki
        public static void EditWiki(string wikiId, string wikiTitle, string wikiContent)
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
            XmlUtils.SaveXML(WikiXML, _xmlFilePath);
        }

        // Delete wiki
        [WebMethod]
        public static void DeleteWiki(string id)
        {
            WikiXML.Root
                    .Elements("WikiEntries")
                    .Elements()
                    .Where(p => p.Attribute("Id")
                    .Value == id)
                    .Remove();

            // Check if there are tags / categories which are not used 

            WikiXML.Save(_xmlFilePath);
        }
        #endregion Methods
    }
}