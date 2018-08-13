using System;
using System.Collections.Generic;
using System.Data;
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
    public partial class Wiki : System.Web.UI.Page
    {
        protected void Page_Render(object sender, EventArgs e)
        {
            lvWikies.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            XDocument doc = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/Wiki.xml"));
            rptWikiMenu.DataSource = from x in doc.Root.Elements("Categories")
                                    .Elements("Category")
                                     select new
                                     {
                                         Title = x.Attribute("Title").Value,
                                         Id = x.Attribute("Id").Value,
                                     };
            rptWikiMenu.DataBind();

            string queryParam = Request.QueryString["category"];

            if (queryParam != null)
            {
                if (!XmlUtils.XmlElementExist(WikiAPI.WikiXML, "Categories", "Title", queryParam))
                {
                    Response.Redirect("/");
                }

                var data = from x in doc.Root.Elements("WikiEntries")
                            .Elements()
                            .Where(p => p.Attribute("CategoryIds")
                            .Value.Contains(queryParam))
                           select new
                           {
                               Title = x.Element("Title").Value,
                               Content = x.Element("Content").Value,
                               WikiId = x.Attribute("Id").Value,
                            };

                lvWikies.DataSource = data.ToList();
                lvWikies.DataBind();
                phQueryResults.Visible = true;
                lblResults.Text = "Results found: " + data.Count();
            }

            string showParam = Request.QueryString["show"];

            if (showParam != null)
            {
                if (!XmlUtils.XmlElementExist(WikiAPI.WikiXML, "WikiEntries", "Id", showParam))
                {
                    Response.Redirect("/");
                }

                var data = from x in doc.Root.Elements("WikiEntries")
                            .Elements()
                            .Where(p => p.Attribute("Id")
                            .Value == showParam)
                           select new
                           {
                               WikiId = x.Attribute("Id").Value,
                               Title = x.Element("Title").Value,
                               Content = CommonMark.CommonMarkConverter.Convert(x.Element("Content").Value),
                           };

                rptWikiEntry.DataSource = data;
                rptWikiEntry.DataBind();
                phWikiEntries.Visible = false;
            }
        }

        [WebMethod]
        public static void DeleteWiki(string id)
        {
            WikiAPI.DeleteWiki(id);
        }
    }
}
