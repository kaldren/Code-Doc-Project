using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TagsCategoriesTest.App_Code.Utils;
using TagsCategoriesTest.App_Code.Wiki;

namespace TagsCategoriesTest
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Bind Categories and Tags Data
            if (!IsPostBack)
            {
                DataUtils.BindCategories(WikiAPI.WikiXML, lbCategories);
                DataUtils.BindTags(WikiAPI.WikiXML, lbTags);
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            // Get data
            WikiDTO wikiDto = new WikiDTO(
                    txtTitle.Text,
                    txtContent.Text, "JohnDoe",
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow,
                    "JohnDoe",
                    DataUtils.GetUniqueData(lbCategories, txtCategories),
                    DataUtils.GetUniqueData(lbTags, txtTags),
                    WikiAPI.WikiRequestType.New
                );

            // Add new entry
            WikiAPI wikiAPI = new WikiAPI();
            wikiAPI.CreateEntry(wikiDto);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            code.Text = DataUtils.PreviewCode(txtContent.Text);
        }
    }
}