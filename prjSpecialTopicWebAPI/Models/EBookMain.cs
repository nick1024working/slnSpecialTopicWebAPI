using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class EBookMain
{
    public long EbookId { get; set; }

    public string EbookName { get; set; } = null!;

    public string? EBookType { get; set; }

    public string Author { get; set; } = null!;

    public string? Publisher { get; set; }

    public DateOnly? PublishedDate { get; set; }

    public string? Translator { get; set; }

    public string? Language { get; set; }

    public string? Isbn { get; set; }

    public string? Eisbn { get; set; }

    public string? PublishedCountry { get; set; }

    public string EBookPosition { get; set; } = null!;

    public string EBookDataType { get; set; } = null!;

    public decimal FixedPrice { get; set; }

    public decimal? ActualPrice { get; set; }

    public string? Couponcode { get; set; }

    public decimal? Discount { get; set; }

    public string? PurchaseCountry { get; set; }

    public long? Cumulativesales { get; set; }

    public string? BookDescription { get; set; }

    public long Weeksales { get; set; }

    public long Monthsales { get; set; }

    public long Totalsales { get; set; }

    public long Weekviews { get; set; }

    public long Monthviews { get; set; }

    public long Totalviews { get; set; }

    public byte MaturityRating { get; set; }

    public bool IsAvailable { get; set; }

    public int CategoryId { get; set; }

    public long? SeriesId { get; set; }

    public string? PrimaryCoverPath { get; set; }

    public virtual EBookCategory Category { get; set; } = null!;

    public virtual ICollection<EBookImage> EBookImages { get; set; } = new List<EBookImage>();

    public virtual ICollection<EbookPurchased> EbookPurchaseds { get; set; } = new List<EbookPurchased>();

    public virtual ICollection<EbookRecommend> EbookRecommends { get; set; } = new List<EbookRecommend>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Series? Series { get; set; }

    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();
}
