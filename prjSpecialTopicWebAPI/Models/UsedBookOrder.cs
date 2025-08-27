using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class UsedBookOrder
{
    public int Id { get; set; }

    public string OrderNo { get; set; } = null!;

    public byte OrderStatus { get; set; }

    public byte PaymentStatus { get; set; }

    public byte DeliveryStatus { get; set; }

    public byte PaymentMethod { get; set; }

    public byte DeliveryMethod { get; set; }

    public Guid BuyerId { get; set; }

    public Guid SellerId { get; set; }

    public Guid BookId { get; set; }

    public string Title { get; set; } = null!;

    public decimal SalePrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual UsedBook Book { get; set; } = null!;

    public virtual User Buyer { get; set; } = null!;

    public virtual User Seller { get; set; } = null!;
}
