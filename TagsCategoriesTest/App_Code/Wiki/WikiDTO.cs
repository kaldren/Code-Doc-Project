using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for WikiDTO
/// </summary>
/// 
namespace TagsCategoriesTest.App_Code.Wiki
{
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

        public WikiDTO(string title, string content, string createdby,
            DateTimeOffset createdat, DateTimeOffset updatedat, string updatedby, HashSet<string> categories, HashSet<string> tags)
        {
            Title = title;
            Content = content;
            CreatedBy = createdby;
            CreatedAt = createdat;
            UpdatedAt = updatedat;
            UpdatedBy = updatedby;
            Categories = categories;
            Tags = tags;
        }

    }
}