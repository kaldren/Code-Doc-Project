using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Web;
using System.Threading;

/// <summary>
/// Summary description for WikiAPI
/// </summary>
public class WikiAPI
{
    private readonly string XML_FILE_PATH = HttpContext.Current.Server.MapPath("~/App_Data/Wiki.xml");
    private XDocument wikiRoot = null;
    private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

    public WikiAPI(){ }

    public void CreateEntry(WikiDTO wiki)
    {
        // Lock 
        _readWriteLock.EnterWriteLock();

        try
        {
            wikiRoot = XDocument.Load(XML_FILE_PATH);
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
                wikiRoot.Root.Element(xmlParent).Add(new XElement(xmlChild, new XAttribute("Id", "66"), new XAttribute(xmlChildAttribute, item)));
            }
        }
    }

    private void AddWikiEntry(WikiDTO wiki)
    {
        XElement wikiEntry =
                new XElement("WikiEntry",
                    new XAttribute("Id", Guid.NewGuid()),
                    new XAttribute("CreatedBy", "kaloyan@kukui.com"),
                    new XAttribute("CreatedAt", DateTimeOffset.UtcNow),
                    new XAttribute("UpdatedBy", "kaloyan@kukui.com"),
                    new XAttribute("UpdatedAt", DateTimeOffset.UtcNow),
                    new XAttribute("CategoryIds", FilterHashData(wiki.Categories, "Categories", "Id")),
                    new XAttribute("TagIds", FilterHashData(wiki.Tags, "Tags", "Id")),
                    new XElement("Title", wiki.Title),
                    new XElement("Content", new XCData(wiki.Content))
                );
        wikiRoot.Root.Element("WikiEntries").Add(wikiEntry);
        wikiRoot.Save(XML_FILE_PATH);
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

    private List<XElement> GetChildNodes(string rootNode, string parentNode, string childNode)
    {
        return wikiRoot.Element("Wiki").Element(parentNode).Elements(childNode).ToList();
    }

    // Filter Data
    private string FilterTags(HashSet<string> tags)
    {
        HashSet<string> tagsFiltered = new HashSet<string>();

        foreach (var tag in tags)
        {
            if (!XmlElementExist("Tags", "Id", tag))
            {
                tagsFiltered.Add(tag);
            }
        }

        return string.Join(",", tagsFiltered);
    }

    private string FilterCategories(HashSet<string> categories)
    {
        HashSet<string> categoriesFiltered = new HashSet<string>();

        foreach (var cat in categories)
        {
            if (!XmlElementExist("Categories", "Id", cat))
            {
                categoriesFiltered.Add(cat);
            }
        }

        return string.Join(",", categoriesFiltered);
    }

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
}