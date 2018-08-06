using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace TagsCategoriesTest
{
    public partial class Wiki : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            XDocument doc = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/Wiki.xml"));
            rptWikiMenu.DataSource = from x in doc.Root.Elements("Categories")
                                    .Elements("Category")
                                     select new
                                     {
                                         Title = x.Attribute("Title").Value,
                                         Id = x.Attribute("Id").Value

                                     };


            rptWikiMenu.DataBind();
        }
    }
}