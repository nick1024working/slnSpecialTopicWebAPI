using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class PostLike
{
    public int LikeId { get; set; }

    public int PostId { get; set; }

    public Guid Uid { get; set; }

    public virtual ForumPost Post { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
