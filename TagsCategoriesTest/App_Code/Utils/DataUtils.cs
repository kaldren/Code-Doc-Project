using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace TagsCategoriesTest.App_Code.Utils
{
    public static class DataUtils
    {
        // Return unique data only
        public static HashSet<string> GetUniqueData(ListBox listBoxData, TextBox textBoxData)
        {
            ListBox listData = listBoxData;
            string[] textData = string.IsNullOrEmpty(textBoxData.Text) ? null : textBoxData.Text.Split(',');

            HashSet<string> uniqueData = new HashSet<string>();

            if (listData.GetSelectedIndices().Count() > 0)
            {
                foreach (ListItem item in listData.Items)
                {
                    if (item.Selected)
                    {
                        uniqueData.Add(item.Value.ToLower().Trim());
                    }
                }
            }

            if (textData != null)
            {
                foreach (var item in textData)
                {
                    uniqueData.Add(item.ToLower().Trim());
                }
            }

            return uniqueData;
        }

        // Preview code
        public static string PreviewCode(string code)
        {
            return CommonMark.CommonMarkConverter.Convert(code);
        }

        // Bind Categories 
        public static void BindCategories(XDocument doc, ListBox categoryId)
        {
            categoryId.DataSource = doc.Root
                                        .Elements("Categories")
                                        .Elements()
                                        .Select(c => c.Attribute("Title").Value)
                                        .ToList();

            categoryId.DataBind();
        }

        // Bind Tags 
        public static void BindTags(XDocument doc, ListBox tagsId)
        {
            tagsId.DataSource = doc.Root
                                    .Elements("Tags")
                                    .Elements()
                                    .Select(c => c.Attribute("Title").Value)
                                    .ToList();

            tagsId.DataBind();
        }
    }
}