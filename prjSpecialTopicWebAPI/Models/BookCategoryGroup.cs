using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class BookCategoryGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public string Slug { get; set; } = null!;

    public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}
