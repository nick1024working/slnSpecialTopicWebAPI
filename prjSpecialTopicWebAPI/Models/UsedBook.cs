using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class UsedBook
{
    public Guid Id { get; set; }

    public Guid SellerId { get; set; }

    public int SellerDistrictId { get; set; }

    public decimal SalePrice { get; set; }

    public string? Title { get; set; }

    public string? Authors { get; set; }

    public int ConditionRatingId { get; set; }

    public string? ConditionDescription { get; set; }

    public string? Edition { get; set; }

    public string? Publisher { get; set; }

    public DateOnly? PublicationDate { get; set; }

    public string? Isbn { get; set; }

    public int? BindingId { get; set; }

    public int? LanguageId { get; set; }

    public int? Pages { get; set; }

    public int ContentRatingId { get; set; }

    public bool IsOnShelf { get; set; }

    public bool IsSold { get; set; }

    public bool IsActive { get; set; }

    public string Slug { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual BookBinding? Binding { get; set; }

    public virtual BookConditionRating ConditionRating { get; set; } = null!;

    public virtual ContentRating ContentRating { get; set; } = null!;

    public virtual Language? Language { get; set; }

    public virtual User Seller { get; set; } = null!;

    public virtual District SellerDistrict { get; set; } = null!;

    public virtual ICollection<UsedBookImage> UsedBookImages { get; set; } = new List<UsedBookImage>();

    public virtual ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();

    public virtual ICollection<BookConditionDetail> Conditions { get; set; } = new List<BookConditionDetail>();

    public virtual ICollection<BookSaleTag> Tags { get; set; } = new List<BookSaleTag>();
}
