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
            rptWikiMenu.DataSource = from x in doc.Root.Elements("WikiEntries")
                                    .Elements("WikiEntry")
                                     select new
                                     {
                                         Title = x.Element("Title").Value,
                                         Id = x.Attribute("Id").Value,
                                         Content = CommonMark.CommonMarkConverter.Convert(x.Element("Content").Value)
                                     };
            rptWikiMenu.DataBind();
        }
    }
}

//https://stackoverflow.com/questions/2923137/repeater-in-repeater