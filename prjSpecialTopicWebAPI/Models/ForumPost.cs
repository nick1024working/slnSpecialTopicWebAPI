using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class ForumPost
{
    public int PostId { get; set; }

    public Guid Uid { get; set; }

    public int PostCategoryId { get; set; }

    public int FilterId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int ViewCount { get; set; }

    public int LikeCount { get; set; }

    public int CommentCount { get; set; }

    public bool IsDeleted { get; set; }

    public virtual PostFilter Filter { get; set; } = null!;

    public virtual ICollection<PostBookmark> PostBookmarks { get; set; } = new List<PostBookmark>();

    public virtual PostCategory PostCategory { get; set; } = null!;

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual PostImage? PostImage { get; set; }

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual User UidNavigation { get; set; } = null!;
}
