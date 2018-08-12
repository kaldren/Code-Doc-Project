using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using TagsCategoriesTest.App_Code.Wiki;

namespace TagsCategoriesTest.App_Code.Utils
{
    public static class XmlUtils
    {
        private static XElement GetAttributeValue(XDocument doc, string element, string attr, string attrValue)
        {
            var node = doc.Descendants(element).Where(p => p.Attribute(attr).Value == attrValue).FirstOrDefault();

            if (node == null)
            {
                return null;
            }

            return node;
        }

        #region Methods
        // Checks if given value exists in the given xml node
        public static bool XmlElementExist(XDocument doc, string parentNode, string attrName, string attrValue)
        {
            XElement data = null;

            try
            {
                data =
                    doc.Root.Elements(parentNode)
                    .Elements()
                    .Where(p => p.Attribute(attrName).Value == attrValue)
                    .FirstOrDefault();
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Wrong XML naming format (Pascal Case required).");
                return false;
            }


            return (data == null) ? false : true;
        }

        // Gets the last node's Id (could increment it by increment)
        public static string GenerateNodeId(XDocument doc, string xmlParent, string xmlChild, string xmlChildAttribute, string xmlAttributeValue, int increment = 1)
        {
            // Create default node if the node list is empty
            if (!XmlElementExist(doc, xmlParent, "Id", "1"))
            {
                return "1";
            }
            else
            {
                var data =
                    doc.Root.Elements(xmlParent)
                    .Elements()
                    .Last()
                    .Attribute("Id").Value;

                int newId = -999;
                var newdata = Int32.TryParse(data, out newId);

                if (!newdata)
                {
                    throw new ArgumentException(data + " is not a numeric string");
                }

                newId += increment;

                return newId.ToString();
            }
        }

        // Filter data
        public static string FilterHashData(XDocument doc, HashSet<string> data, string xmlParent, string xmlChildAttribute)
        {
            HashSet<string> filteredData = new HashSet<string>();

            foreach (var tag in data)
            {
                if (!XmlUtils.XmlElementExist(doc, xmlParent, xmlChildAttribute, tag))
                {
                    filteredData.Add(tag);
                }
            }

            return string.Join(",", filteredData);
        }

        // Get node by attribute
        public static XElement GetNodeByAttributeValue (XDocument doc, string parentNode, string childAttribute, string expectedValue)
        {
            if (!string.IsNullOrEmpty(expectedValue))
            {
                return doc.Root
                        .Elements(parentNode)
                        .Elements()
                        .Where(p => p.Attribute(childAttribute)
                        .Value == expectedValue)
                        .FirstOrDefault();
            }

            return null;
        }

        // Update the references
        public static string UpdateReferences(XDocument doc, string parentNode, string title, int factor)
        {
            // Create default node if the node list is empty
            if (!XmlElementExist(doc, parentNode, "Title", title))
            {
                return "1";
            }
            else
            {
                var data = doc.Root
                            .Elements(parentNode)
                            .Elements()
                            .Where(p => p.Attribute("Title").Value == title)
                            .Select(p => (string)p.Attribute("Referenced").Value).ToString();


                int newReference = -999;
                var newdata = Int32.TryParse(data, out newReference);

                if (!newdata)
                {
                    throw new ArgumentException(data + " is not a numeric string");
                }

                newReference += factor;

                return newReference.ToString();
            }
        }

        // Add new WikiEntry
        public static void AddWikiEntry(XDocument doc, WikiDTO wiki)
        {
            XElement wikiEntry =
                    new XElement("WikiEntry",
                        new XAttribute("Id", Guid.NewGuid()),
                        new XAttribute("CreatedBy", "kaloyan@kukui.com"),
                        new XAttribute("CreatedAt", DateTimeOffset.UtcNow.ToString()),
                        new XAttribute("UpdatedBy", "kaloyan@kukui.com"),
                        new XAttribute("UpdatedAt", DateTimeOffset.UtcNow.ToString()),
                        new XAttribute("CategoryIds", XmlUtils.FilterHashData(doc, wiki.Categories, "Categories", "Id")),
                        new XAttribute("TagIds", XmlUtils.FilterHashData(doc, wiki.Tags, "Tags", "Id")),
                        new XElement("Title", wiki.Title),
                        new XElement("Content", new XCData(wiki.Content))
                    );
            doc.Root.Element("WikiEntries").Add(wikiEntry);
            doc.Save(WikiAPI.XmlFilePath);
        }

        // Edit WikiEntry
        public static void EditWikiEntry(XDocument doc, string wikiId, string wikiTitle, string wikiContent)
        {
            var data = doc.Root
                    .Elements("WikiEntries")
                    .Elements()
                    .Where(p => p.Attribute("Id")
                    .Value == wikiId)
                    .FirstOrDefault();

            data.Element("Title").Value = wikiTitle;
            data.Element("Content").Value = wikiContent;
            data.Attribute("UpdatedAt").Value = DateTimeOffset.UtcNow.ToString();
            XmlUtils.SaveXML(doc, WikiAPI.XmlFilePath);
        }

        // Delete WikiEntry
        public static void DeleteWikiEntry(XDocument doc, string id)
        {
            var delEntry = doc.Root
                    .Elements("WikiEntries")
                    .Elements()
                    .Where(p => p.Attribute("Id")
                    .Value == id)
                    .FirstOrDefault();

            FilterReferences(doc, delEntry);
            delEntry.Remove();
            doc.Save(WikiAPI.XmlFilePath);
        }

        public static void RemoveElement(XDocument doc, string parentElement, string childAttr, string childValue)
        {
            doc.Descendants(parentElement)
                .Where(p => p.Attribute(childAttr).Value == childValue).Remove();
        }

        public static void UpdateReference(XDocument doc, string parentElement, string childAttr, string childValue)
        {
            var data = doc.Descendants(parentElement)
                            .Where(item => item.Attribute(childAttr).Value == childValue)
                            .ToList();

            foreach (var item in data)
            {
                item.Attribute("Referenced").Value = (Convert.ToInt32(item.Attribute("Referenced").Value) - 1).ToString();
            }
        }

        private static void FilterReferences(XDocument doc, XElement element)
        {

            // Filter Categories
            var tempCategories = element.Attribute("CategoryIds").Value;
            var filteredCategories = tempCategories.Split(',');
            for (int i = 0; i < filteredCategories.Length; i++)
            {
                var tempy = doc.Descendants("Categories")
                            .Elements()
                            .Where(p => p.Attribute("Title").Value == filteredCategories[i])
                            .Select(p => p.Attribute("Referenced").Value).FirstOrDefault();

                if (tempy == "1")
                {
                    RemoveElement(WikiAPI.WikiXML, "Category", "Title", filteredCategories[i]);
                }
                else
                {
                    UpdateReference(WikiAPI.WikiXML, "Category", "Title", filteredCategories[i]);
                }
            }

            // Filter Tags
            var tempTags = element.Attribute("TagIds").Value;
            var filteredTags = tempCategories.Split(',');
            for (int i = 0; i < filteredCategories.Length; i++)
            {
                var tempy = doc.Descendants("Tags")
                            .Elements()
                            .Where(p => p.Attribute("Title").Value == filteredCategories[i])
                            .Select(p => p.Attribute("Referenced").Value).FirstOrDefault();

                if (tempy == "1")
                {
                    RemoveElement(WikiAPI.WikiXML, "Tag", "Title", filteredCategories[i]);
                }
                else
                {
                    UpdateReference(WikiAPI.WikiXML, "Tag", "Title", filteredCategories[i]);
                }
            }

        }
        // Save XML data
        public static void SaveXML(XDocument doc, string filepath)
        {
            doc.Save(filepath);
        }
        #endregion Methods
    }
}