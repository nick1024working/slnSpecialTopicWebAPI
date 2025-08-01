using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class PostComment
{
    public int CommentId { get; set; }

    public int PostId { get; set; }

    public int? ParentCommentId { get; set; }

    public Guid Uid { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<PostComment> InverseParentComment { get; set; } = new List<PostComment>();

    public virtual PostComment? ParentComment { get; set; }

    public virtual ForumPost Post { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}
