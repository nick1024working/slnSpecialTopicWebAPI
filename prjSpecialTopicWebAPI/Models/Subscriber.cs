using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class Subscriber
{
    public long SubscriptionId { get; set; }

    public Guid Uid { get; set; }

    public int PlanId { get; set; }

    public DateTime? DueTime { get; set; }

    public DateTime? LastPayTime { get; set; }

    public DateTime? NextPayTime { get; set; }

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
