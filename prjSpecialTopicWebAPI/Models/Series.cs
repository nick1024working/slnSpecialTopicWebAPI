using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class Series
{
    public long SeriesId { get; set; }

    public string SeriesName { get; set; } = null!;

    public string? SeriesDescription { get; set; }

    public bool IsComplete { get; set; }

    public int? TotalVolumes { get; set; }

    public virtual ICollection<EBookMain> EBookMains { get; set; } = new List<EBookMain>();
}
