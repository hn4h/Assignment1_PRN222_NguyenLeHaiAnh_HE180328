using System;
using System.Collections.Generic;

namespace NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public string? TagName { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
