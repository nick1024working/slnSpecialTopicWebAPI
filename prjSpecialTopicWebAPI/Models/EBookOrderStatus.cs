using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class EBookOrderStatus
{
    public int OrderStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsTerminal { get; set; }

    public int? SortOrder { get; set; }

    public bool? AllowUserAction { get; set; }

    public virtual ICollection<EBookOrderMain> EBookOrderMains { get; set; } = new List<EBookOrderMain>();
}
