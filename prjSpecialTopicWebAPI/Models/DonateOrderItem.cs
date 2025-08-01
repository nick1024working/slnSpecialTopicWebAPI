using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class DonateOrderItem
{
    public int OrderItemsId { get; set; }

    public int DonateOrderId { get; set; }

    public int DonatePlanId { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }

    public virtual DonateOrder DonateOrder { get; set; } = null!;

    public virtual DonatePlan DonatePlan { get; set; } = null!;
}
