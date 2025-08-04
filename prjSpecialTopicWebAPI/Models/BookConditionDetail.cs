using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class BookConditionDetail
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<UsedBook> Books { get; set; } = new List<UsedBook>();
}
