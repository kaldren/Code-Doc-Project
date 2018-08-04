using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for WikiDTO
/// </summary>
/// 

public class WikiDTO
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public string UpdatedBy { get; private set; }
    public HashSet<string> Categories { get; private set; }
    public HashSet<string> Tags { get; private set; }

    public WikiDTO(string id, string title, string content, string createdby, 
        DateTimeOffset createdat, DateTimeOffset updatedat, string updatedby, HashSet<string> categories, HashSet<string> tags)
    {
        Id = id;
        Title = title;
        Content = content;
        CreatedBy = createdby;
        CreatedAt = createdat;
        UpdatedAt = updatedat;
        UpdatedBy = updatedby;
        Categories = categories;
        Tags = tags;
    }

    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public static HashSet<string> GetUniqueData(ListBox listBoxData, TextBox textBoxData)
    {
        ListBox listData = listBoxData;
        string[] textData = string.IsNullOrEmpty(textBoxData.Text) ? null : textBoxData.Text.Split(',');

        HashSet<string> uniqueDataList = new HashSet<string>();

        if (listData.GetSelectedIndices().Count() > 0)
        {
            foreach (ListItem item in listData.Items)
            {
                if (item.Selected)
                {
                    uniqueDataList.Add(item.Value.ToLower().Trim());
                }
            }
        }

        if (textData != null)
        {
            foreach (var item in textData)
            {
                uniqueDataList.Add(item.ToLower().Trim());
            }
        }

        return uniqueDataList;
    }
}