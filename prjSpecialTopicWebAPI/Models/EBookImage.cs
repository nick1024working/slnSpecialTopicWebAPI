using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class EBookImage
{
    public long ImageId { get; set; }

    public long EBookId { get; set; }

    public string ImagePath { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public int? SortOrder { get; set; }

    public virtual EBookMain EBook { get; set; } = null!;
}
