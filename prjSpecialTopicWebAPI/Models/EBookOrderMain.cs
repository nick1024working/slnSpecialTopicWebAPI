using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class EBookOrderMain
{
    public long OrderId { get; set; }

    public Guid Uid { get; set; }

    public DateTime OrderDateTime { get; set; }

    public int OrderStatusId { get; set; }

    public decimal TotalAmount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string? PaymentGatewayTransactionId { get; set; }

    public int? PaymentMethodId { get; set; }

    public string? Ipaddress { get; set; }

    public string? UserAgent { get; set; }

    public long? BillingAddressId { get; set; }

    public decimal? TotalDiscountAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual EBookOrderStatus OrderStatus { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
