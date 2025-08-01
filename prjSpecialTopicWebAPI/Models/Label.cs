using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class Label
{
    public int LabelId { get; set; }

    public string LabelName { get; set; } = null!;

    public virtual ICollection<EBookMain> EBooks { get; set; } = new List<EBookMain>();
}
