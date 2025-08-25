using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class BookCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public string Slug { get; set; } = null!;

    public virtual ICollection<UsedBook> UsedBooks { get; set; } = new List<UsedBook>();
}
