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
    }
}