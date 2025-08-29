using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class UsedBookOrder
{
    public int Id { get; set; }

    public string OrderNo { get; set; } = null!;

    public Guid BuyerId { get; set; }

    public Guid SellerId { get; set; }

    public byte OrderStatus { get; set; }

    public byte PaymentStatus { get; set; }

    public byte DeliveryStatus { get; set; }

    public byte PaymentMethod { get; set; }

    public byte DeliveryMethod { get; set; }

    public string? TransactionId { get; set; }

    public string? TrackingNumber { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountTotal { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal GrandTotal { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User Buyer { get; set; } = null!;

    public virtual User Seller { get; set; } = null!;

    public virtual ICollection<UsedBookOrderItem> UsedBookOrderItems { get; set; } = new List<UsedBookOrderItem>();
}
