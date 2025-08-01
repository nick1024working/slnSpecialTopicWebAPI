using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class EBookCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int? ParentCategoryId { get; set; }

    public virtual ICollection<EBookMain> EBookMains { get; set; } = new List<EBookMain>();

    public virtual ICollection<EBookCategory> InverseParentCategory { get; set; } = new List<EBookCategory>();

    public virtual EBookCategory? ParentCategory { get; set; }
}
