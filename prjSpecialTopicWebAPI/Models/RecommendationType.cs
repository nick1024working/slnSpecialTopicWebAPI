using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class RecommendationType
{
    public int RecTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<EbookRecommend> EbookRecommends { get; set; } = new List<EbookRecommend>();
}
