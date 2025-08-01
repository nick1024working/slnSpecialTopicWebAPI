using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class SubscriptionPlan
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public string Scope { get; set; } = null!;

    public int DurationDays { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Subscriber> Subscribers { get; set; } = new List<Subscriber>();
}
