using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TagsCategoriesTest.App_Code.Wiki;
using static TagsCategoriesTest.App_Code.Wiki.WikiAPI;

namespace TagsCategoriesTest.App_Code.Utils
{
    public static class XmlUtils
    {
        private static XElement GetAttributeValue(XDocument doc, string element, string attr, string attrValue)
        {
            var node = doc.Descendants(element).Where(p => p.Attribute(attr).Value == attrValue).FirstOrDefault();

            return node;
        }

        #region Methods
        // Checks if given value exists in the given xml node
        public static bool XmlElementExist(XDocument doc, string parentNode, string attrName, string attrValue)
        {
            var data = doc.Descendants(parentNode)
                .Elements()
                .Where(p => p.Attribute(attrName).Value == attrValue)
                .FirstOrDefault();

            return data != null;
        }

        // Gets the last node Id (could increment it by increment)
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

        public static void AddWikiNodes(HashSet<string> data, string xmlParent, string xmlChild, string xmlChildAttribute, WikiRequestType wikiRequestType)
        {
            foreach (var item in data)
            {
                // Create new Tag / Category 
                if (!XmlUtils.XmlElementExist(WikiAPI.WikiXML, xmlParent, xmlChildAttribute, item))
                {
                    WikiAPI.WikiXML.Root
                            .Element(xmlParent)
                            .Add(
                                new XElement(xmlChild,
                                new XAttribute("Id", XmlUtils.GenerateNodeId(WikiAPI.WikiXML, xmlParent, xmlChild, xmlChildAttribute, item)),
                                new XAttribute(xmlChildAttribute, item),
                                new XAttribute("Referenced", XmlUtils.UpdateReferences(WikiAPI.WikiXML, xmlParent, xmlChild, 1)))
                            );
                }
                else
                {
                    if (wikiRequestType == WikiRequestType.New)
                    {
                        var node = WikiAPI.WikiXML.Descendants(xmlParent).Elements(xmlChild).Where(p => p.Attribute("Title").Value == item).FirstOrDefault();

                        if (node != null)
                        {
                            node.Attribute("Referenced").Value = (Convert.ToInt32(node.Attribute("Referenced").Value) + 1).ToString();
                        }
                    }
                }
            }
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
        public static void EditWikiEntry(WikiDTO wiki, string wikiId)
        {
            var data = WikiAPI.WikiXML.Root
                    .Elements("WikiEntries")
                    .Elements()
                    .Where(p => p.Attribute("Id")
                    .Value == wikiId)
                    .FirstOrDefault();

            // Get previous TagId / CategoryIds

            var oldTags = data.Attribute("TagIds").Value.Split(',');
            var newTags = XmlUtils.FilterHashData(WikiAPI.WikiXML, wiki.Tags, "Tags", "Id").Split(',');

            var oldCategories = data.Attribute("CategoryIds").Value.Split(',');
            var newCategories = XmlUtils.FilterHashData(WikiAPI.WikiXML, wiki.Categories, "Categories", "Id").Split(',');

            foreach (var tag in oldTags)
            {
                // The element no longer exists in the new input data - decrease it's reference value (or remove it completely)
                if (Array.IndexOf(newTags, tag) <= -1)
                {
                    UpdateReferencev2("Tags", "Title", tag, -1);
                }
                else
                {
                    // The element exists, increase reference
                    UpdateReferencev2("Tags", "Title", tag, 0);
                }
            }

            foreach (var category in oldCategories)
            {
                // The element no longer exists in the new input data - decrease it's reference value (or remove it completely)
                if (Array.IndexOf(newCategories, category) <= -1)
                {
                    UpdateReferencev2("Categories", "Title", category, -1);
                }
                else
                {
                    // The element exists, upte increase reference
                    UpdateReferencev2("Categories", "Title", category, 0);
                }
            }

            if (data != null)
            {
                XmlUtils.AddWikiNodes(wiki.Categories, "Categories", "Category", "Title", WikiRequestType.Edit);
                XmlUtils.AddWikiNodes(wiki.Tags, "Tags", "Tag", "Title", WikiRequestType.Edit);

                data.Element("Title").Value = wiki.Title;
                data.Element("Content").Value = wiki.Content;
                data.Attribute("CategoryIds").Value = XmlUtils.FilterHashData(WikiAPI.WikiXML, wiki.Categories, "Categories", "Id");
                data.Attribute("TagIds").Value = XmlUtils.FilterHashData(WikiAPI.WikiXML, wiki.Tags, "Tags", "Id");
                data.Attribute("UpdatedAt").Value = wiki.UpdatedAt.ToString();
                XmlUtils.SaveXML(WikiAPI.WikiXML, WikiAPI.XmlFilePath);
            }
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

            if (delEntry != null)
            {
                FilterReferences(doc, delEntry);
                delEntry.Remove();
                doc.Save(WikiAPI.XmlFilePath);
            }
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

            if (data != null)
            {
                foreach (var item in data)
                {
                    item.Attribute("Referenced").Value = (Convert.ToInt32(item.Attribute("Referenced").Value) - 1).ToString();
                }
            }
        }

        public static void UpdateReferencev2(string parentElement, string title, string value, int factor)
        {
            var data = WikiAPI.WikiXML.Root
                    .Elements(parentElement)
                    .Elements()
                    .Where(item => item.Attribute(title).Value == value)
                    .FirstOrDefault();

            if (data.Attribute("Referenced").Value == "1")
            {
                if (factor < 1)
                {
                    // Remove the attribute since it is no longer referenced
                    data.Remove();
                }
                else
                {
                    data.Attribute("Referenced").Value = (Convert.ToInt32(data.Attribute("Referenced").Value) + factor).ToString();
                }
            }
            else
            {
                data.Attribute("Referenced").Value = (Convert.ToInt32(data.Attribute("Referenced").Value) + factor).ToString();
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
            var filteredTags = tempTags.Split(',');

            for (int i = 0; i < filteredTags.Length; i++)
            {
                var tempy = doc.Descendants("Tags")
                            .Elements()
                            .Where(p => p.Attribute("Title").Value == filteredTags[i])
                            .Select(p => p.Attribute("Referenced").Value).FirstOrDefault();

                if (tempy == "1")
                {
                    RemoveElement(WikiAPI.WikiXML, "Tag", "Title", filteredTags[i]);
                }
                else
                {
                    UpdateReference(WikiAPI.WikiXML, "Tag", "Title", filteredTags[i]);
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