using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class Language
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<UsedBook> UsedBooks { get; set; } = new List<UsedBook>();
}
