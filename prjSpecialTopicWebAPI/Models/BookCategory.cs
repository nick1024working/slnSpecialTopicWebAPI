using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class BookCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public virtual ICollection<BookCategory> InverseParent { get; set; } = new List<BookCategory>();

    public virtual BookCategory? Parent { get; set; }

    public virtual ICollection<UsedBook> Books { get; set; } = new List<UsedBook>();
}
