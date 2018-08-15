using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TagsCategoriesTest.App_Code.Utils;
using TagsCategoriesTest.App_Code.Wiki;
using static TagsCategoriesTest.App_Code.Wiki.WikiAPI;

namespace TagsCategoriesTest
{
    public partial class Edit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataUtils.BindCategories(WikiAPI.WikiXML, lbCategories);
                DataUtils.BindCategories(WikiAPI.WikiXML, lbTags);


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
                    txtCategories.Text = wiki.Attribute("CategoryIds").Value;
                    txtTags.Text = wiki.Attribute("TagIds").Value;
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
            WikiAPI.EditWiki(new WikiDTO(
                    txtTitle.Text,
                    txtContent.Text, "JohnDoe",
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow,
                    "JohnDoe",
                    DataUtils.GetUniqueData(lbCategories, txtCategories),
                    DataUtils.GetUniqueData(lbTags, txtTags),
                    WikiRequestType.Edit
                ), wikiId.Value);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            code.Text = DataUtils.PreviewCode(txtContent.Text);
        }
    }
}