using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class ItemType
{
    public int ItemTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
