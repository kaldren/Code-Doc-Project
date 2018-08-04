using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace TagsCategoriesTest
{
    public partial class Default : System.Web.UI.Page
    {
        //private readonly string XML_FILE_PATH = @"~\App_Data\Wiki.xml";
        //XDocument wiki = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //XDocument doc = XDocument.Load(@"C:\Users\KDRENSKI\Desktop\TagsCategoriesTest\TagsCategoriesTest\TagsCategoriesTest\App_Data\Wiki.xml");

            //var data = doc.Root.Elements("WikiEntries")
            //    .Elements()
            //    .Where(p => p.Attribute("Id")
            //    .Value == "5c5b4189-e11b-45ca-9904-03b80fae473a")
            //    .Select(p => p.Element("Content").Value).FirstOrDefault();

            //code.Text = CommonMark.CommonMarkConverter.Convert(data);
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            // Get data
            WikiDTO wikiDto = new WikiDTO("5",txtTitle.Text,txtContent.Text,"JohnDoe",DateTimeOffset.UtcNow,DateTimeOffset.UtcNow,"JohnDoe", WikiDTO.GetUniqueData(lbTags, txtTags), WikiDTO.GetUniqueData(lbCategories, txtCategories));

            // Add new entry
            WikiAPI wikiAPI = new WikiAPI();
            wikiAPI.CreateEntry(wikiDto);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            code.Text = CommonMark.CommonMarkConverter.Convert(txtContent.Text);
        }
    }
}