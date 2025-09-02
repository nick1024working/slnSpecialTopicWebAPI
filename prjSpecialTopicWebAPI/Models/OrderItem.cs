using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class OrderItem
{
    public long OrderItemId { get; set; }

    public long OrderId { get; set; }

    public int ItemTypeId { get; set; }

    public long? EBookId { get; set; }

    public long? WorkId { get; set; }

    public long? ChapterId { get; set; }

    public int? PlanId { get; set; }

    public string ItemNameSnapshot { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPriceAtPurchase { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal LineItemTotal { get; set; }

    public virtual EBookMain? EBook { get; set; }

    public virtual ItemType ItemType { get; set; } = null!;

    public virtual EBookOrderMain Order { get; set; } = null!;

    public virtual SubscriptionPlan? Plan { get; set; }
}
