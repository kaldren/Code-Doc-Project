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

        private void AddWikiEntry(WikiDTO wiki)
        {
            // Add all new tags / categories 
            XmlUtils.AddWikiNodes(wiki.Categories, "Categories", "Category", "Title");
            XmlUtils.AddWikiNodes(wiki.Tags, "Tags", "Tag", "Title");
            XmlUtils.AddWikiEntry(WikiXML, wiki);
        }

        // CRUD operations

        // Create wiki
        public void CreateEntry(WikiDTO wiki)
        {
            if (wiki == null)
            {
                throw new ArgumentNullException(nameof(wiki));
                //return;
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
        public static void EditWiki(WikiDTO wiki, string wikiId)
        {
            XmlUtils.EditWikiEntry(wiki, wikiId);
        }

        // Delete wiki
        [WebMethod]
        public static void DeleteWiki(string id)
        {
            XmlUtils.DeleteWikiEntry(WikiXML, id);
        }
        #endregion Methods
    }
}