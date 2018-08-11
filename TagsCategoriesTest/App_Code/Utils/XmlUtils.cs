using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

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
                    .Where(p => p.Attribute(attrName)
                    .Value == attrValue)
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
        public static string GenerateNodeId(XDocument doc, string parentNode, int increment = 1)
        {
            var data =
                doc.Root.Elements(parentNode)
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

        // Filter Data
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

        // Get First Node

        // Save XML Data
        public static void SaveXML(XDocument doc, string filepath)
        {
            doc.Save(filepath);
        }
    }
}