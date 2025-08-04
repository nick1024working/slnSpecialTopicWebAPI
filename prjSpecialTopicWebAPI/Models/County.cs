using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class County
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
