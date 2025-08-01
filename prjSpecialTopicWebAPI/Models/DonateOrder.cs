using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class DonateOrder
{
    public int DonateOrderId { get; set; }

    public Guid Uid { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime? PaymentDate { get; set; }

    public DateTime OrderCreatedAt { get; set; }

    public virtual ICollection<DonateOrderItem> DonateOrderItems { get; set; } = new List<DonateOrderItem>();
}
