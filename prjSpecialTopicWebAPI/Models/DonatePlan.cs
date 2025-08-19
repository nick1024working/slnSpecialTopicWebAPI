using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class DonatePlan
{
    public int DonatePlanId { get; set; }

    public int DonateProjectId { get; set; }

    public string PlanTitle { get; set; } = null!;

    public decimal Price { get; set; }

    public string? PlanDescription { get; set; }

    public virtual ICollection<DonateOrderItem> DonateOrderItems { get; set; } = new List<DonateOrderItem>();

    public virtual DonateProject DonateProject { get; set; } = null!;
}
