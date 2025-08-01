using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class BookConditionRating
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<UsedBook> UsedBooks { get; set; } = new List<UsedBook>();
}
