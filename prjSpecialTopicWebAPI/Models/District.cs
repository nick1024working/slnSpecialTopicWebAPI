using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class District
{
    public int Id { get; set; }

    public int CountyId { get; set; }

    public string Name { get; set; } = null!;

    public virtual County County { get; set; } = null!;

    public virtual ICollection<UsedBook> UsedBooks { get; set; } = new List<UsedBook>();
}
