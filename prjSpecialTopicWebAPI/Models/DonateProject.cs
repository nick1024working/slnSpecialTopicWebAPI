using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class DonateProject
{
    public int DonateProjectId { get; set; }

    public int DonateCategoriesId { get; set; }

    public Guid Uid { get; set; }

    public string ProjectTitle { get; set; } = null!;

    public string? ProjectDescription { get; set; }

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public string? ProjectLongDescription { get; set; }

    public int? BackerCount { get; set; }

    public bool? ProjectIsFavorite { get; set; }

    public virtual DonateCategory DonateCategories { get; set; } = null!;

    public virtual ICollection<DonateImage> DonateImages { get; set; } = new List<DonateImage>();

    public virtual ICollection<DonatePlan> DonatePlans { get; set; } = new List<DonatePlan>();
}
