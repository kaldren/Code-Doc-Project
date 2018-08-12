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

        public static bool ReferenceExist(XDocument doc, string parentNode, string attrTitle, string attrValue)
        {
            XElement data = null;

            try
            {
                data =
                    doc.Root.Elements(parentNode)
                    .Elements()
                    .Where(p => p.Attribute(attrTitle).Value == attrValue)
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

        public static string UpdateReferences(XDocument doc, string parentNode, string title)
        {
            // Create default node if the node list is empty
            if (!ReferenceExist(doc, parentNode, "Title", title))
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

                newReference += 1;

                return newReference.ToString();
            }
        }

        // Save XML data
        public static void SaveXML(XDocument doc, string filepath)
        {
            doc.Save(filepath);
        }
    }
}