using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class UsedBookOrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Guid BookId { get; set; }

    public string Title { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public virtual UsedBook Book { get; set; } = null!;

    public virtual UsedBookOrder Order { get; set; } = null!;
}
