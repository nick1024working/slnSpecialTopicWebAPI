using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class EbookRecommend
{
    public long RecommendId { get; set; }

    public long EbookId { get; set; }

    public int RecTypeId { get; set; }

    public virtual EBookMain Ebook { get; set; } = null!;

    public virtual RecommendationType RecType { get; set; } = null!;
}
