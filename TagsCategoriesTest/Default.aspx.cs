﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace TagsCategoriesTest
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Bind Categories and Tags Data
            if (!IsPostBack)
            {
                WikiAPI.BindCategories(lbCategories);
                WikiAPI.BindTags(lbTags);
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            // Get data
            WikiDTO wikiDto = 
                new WikiDTO(txtTitle.Text,
                txtContent.Text,"JohnDoe",
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                "JohnDoe", 
                WikiDTO.GetUniqueData(lbTags, txtTags), 
                WikiDTO.GetUniqueData(lbCategories, txtCategories));

            // Add new entry
            WikiAPI wikiAPI = new WikiAPI();
            wikiAPI.CreateEntry(wikiDto);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            code.Text = WikiAPI.PreviewCode(txtContent.Text);
        }
    }
}