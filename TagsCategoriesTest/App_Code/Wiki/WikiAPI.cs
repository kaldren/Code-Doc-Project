using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Web;
using System.Threading;
using System.Web.Services;

/// <summary>
/// Summary description for WikiAPI
/// </summary>
public class WikiAPI
{
    public readonly static string XML_FILE_PATH = HttpContext.Current.Server.MapPath(@"~/App_Data/Wiki.xml");
    private static readonly XDocument wikiRoot = XDocument.Load(XML_FILE_PATH);
    private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

    public WikiAPI(){ }

    public void CreateEntry(WikiDTO wiki)
    {
        if (wiki == null)
        {
            throw new ArgumentNullException(nameof(wiki));
        }
        // Lock 
        _readWriteLock.EnterWriteLock();

        try
        {
            // Add all new tags / categories (If they don't already exist in the XML)
            AddWikiNodes(wiki.Categories, "Categories", "Category", "Title");
            AddWikiNodes(wiki.Tags, "Tags", "Tag", "Title");

            // Add Wiki Entry
            AddWikiEntry(wiki);
        }
        finally
        {
            // Release lock
            _readWriteLock.ExitWriteLock();
        }
    }

    private void AddWikiNodes(HashSet<string> data, string xmlParent, string xmlChild, string xmlChildAttribute)
    {
        foreach (var item in data)
        {
            if (!XmlElementExist(xmlParent, xmlChildAttribute, item))
            {
                // Add it
                wikiRoot.Root.Element(xmlParent).Add(new XElement(xmlChild, new XAttribute("Id", GenerateId(xmlParent)), new XAttribute(xmlChildAttribute, item)));
            }
        }
    }

    // Creates WikiEntry
    private void AddWikiEntry(WikiDTO wiki)
    {
        XElement wikiEntry =
                new XElement("WikiEntry",
                    new XAttribute("Id", Guid.NewGuid()),
                    new XAttribute("CreatedBy", "kaloyan@kukui.com"),
                    new XAttribute("CreatedAt", DateTimeOffset.UtcNow.ToString()),
                    new XAttribute("UpdatedBy", "kaloyan@kukui.com"),
                    new XAttribute("UpdatedAt", DateTimeOffset.UtcNow.ToString()),
                    new XAttribute("CategoryIds", FilterHashData(wiki.Categories, "Categories", "Id")),
                    new XAttribute("TagIds", FilterHashData(wiki.Tags, "Tags", "Id")),
                    new XElement("Title", wiki.Title),
                    new XElement("Content", new XCData(wiki.Content))
                );
        wikiRoot.Root.Element("WikiEntries").Add(wikiEntry);
        wikiRoot.Save(XML_FILE_PATH);
    }

    // Increments the Id of the last child node by increment
    private string GenerateId(string parentNode, int increment = 1)
    {
        var data =
            wikiRoot.Root.Elements(parentNode)
            .Elements()
            .Last()
            .Attribute("Id").Value;

        int newId = -999;
        var newdata = Int32.TryParse(data, out newId);

        if (!newdata)
        {
            throw new ArgumentException(data + " is not a numeric string");
        }

        newId += increment;

        return newId.ToString();
    }

    // Filters data and returns unique entries
    private HashSet<string> GetUniqueData(ListBox listBoxData, TextBox textBoxData)
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

    // Checks if given value exists in the given xml node
    private bool XmlElementExist(string parentNode, string attrName, string attrValue)
    {
        XElement data = null;

        try
        {
            data =
                wikiRoot.Root.Elements(parentNode)
                .Elements()
                .Where(p => p.Attribute(attrName)
                .Value == attrValue)
                .FirstOrDefault();
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("Wrong XML naming format (Pascal Case required).");
            return false;
        }


        return (data == null) ? false : true;
    }

    // Filter Data
    private string FilterHashData(HashSet<string> data, string xmlParent, string xmlChildAttribute)
    {
        HashSet<string> filteredData = new HashSet<string>();

        foreach (var tag in data)
        {
            if (!XmlElementExist(xmlParent, xmlChildAttribute, tag))
            {
                filteredData.Add(tag);
            }
        }

        return string.Join(",", filteredData);
    }

    // Show entry by id
    public void ShowEntry(string category)
    {
        var data = from x in wikiRoot.Root.Elements("WikiEntries")
                                    .Elements("WikiEntry")
                                    where x.Attribute("CategoryIds").Value == category
                                    select new
                                    {
                                        Title = x.Attribute("Title").Value,
                                        Id = x.Attribute("Id").Value,
                                        //Content = CommonMark.CommonMarkConverter.Convert(x.Element("Content").Value
                                    };
    }

    // Preview code
    public static string PreviewCode(string code)
    {
        return CommonMark.CommonMarkConverter.Convert(code);
    }

    // Bind Categories 
    public static void BindCategories(ListBox categoryId)
    {
        categoryId.DataSource = wikiRoot
                                    .Root
                                    .Elements("Categories")
                                    .Elements()
                                    .Select(c => c.Attribute("Title").Value)
                                    .ToList();

        categoryId.DataBind();
    }

    // Bind Tags 
    public static void BindTags(ListBox tagsId)
    {
        tagsId.DataSource = wikiRoot
                                .Root
                                .Elements("Tags")
                                .Elements()
                                .Select(c => c.Attribute("Title").Value)
                                .ToList();

        tagsId.DataBind();
    }

    // Load the XML document
    public static XDocument GetXML()
    {
        return XDocument.Load(XML_FILE_PATH);
    }

    // Edit Wiki Entry
    public static void EditEntry(string wikiId, string wikiTitle, string wikiContent)
    {
        var data = GetXML()
                        .Root
                        .Elements("WikiEntries")
                        .Elements()
                        .Where(p => p.Attribute("Id")
                        .Value == wikiId)
                        .FirstOrDefault();

        data.Element("Title").Value = wikiTitle;
        data.Element("Content").Value = wikiContent;
        data.Attribute("UpdatedAt").Value = DateTimeOffset.UtcNow.ToString();
        SaveXML();
    }

    // Save XML Data
    public static void SaveXML()
    {
        wikiRoot.Save(XML_FILE_PATH);
    }
}