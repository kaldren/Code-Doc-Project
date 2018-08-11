using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TagsCategoriesTest.App_Code.Wiki;

namespace TagsCategoriesTest
{
    public partial class Edit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string queryParam = Request.QueryString["id"];

                if (queryParam != null)
                {
                    var doc = WikiAPI.GetXML();

                    var data = doc.Root.Elements("WikiEntries")
                        .Elements()
                        .Where(p => p.Attribute("Id")
                        .Value == queryParam)
                        .FirstOrDefault();

                    txtTitle.Text = data.Element("Title").Value;
                    txtContent.Text = data.Element("Content").Value;
                    wikiId.Value = queryParam;
                }
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            WikiAPI.EditEntry(wikiId.Value, txtTitle.Text, txtContent.Text);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            code.Text = WikiAPI.PreviewCode(txtContent.Text);
        }
    }
}