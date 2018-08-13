using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TagsCategoriesTest.App_Code.Utils;
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

                if (string.IsNullOrEmpty(queryParam))
                {
                    Response.Redirect("/");
                }

                var wiki = WikiAPI.GetWikiById("WikiEntries", queryParam);

                if (wiki != null)
                {
                    txtTitle.Text = wiki.Element("Title").Value;
                    txtContent.Text = wiki.Element("Content").Value;
                    wikiId.Value = queryParam;
                }
                else
                {
                    // Report error message
                    Response.Redirect("/");
                }
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            WikiAPI.EditWiki(wikiId.Value, txtTitle.Text, txtContent.Text);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            code.Text = DataUtils.PreviewCode(txtContent.Text);
        }
    }
}