using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class EbookPurchased
{
    public long PurchaseId { get; set; }

    public Guid Uid { get; set; }

    public long EBookId { get; set; }

    public DateTime PurchaseDateTime { get; set; }

    public DateTime? LastReadTime { get; set; }

    public string? ReadingProgress { get; set; }

    public virtual EBookMain EBook { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
