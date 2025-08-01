using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class PostFilter
{
    public int PostFilterId { get; set; }

    public string FilterName { get; set; } = null!;

    public int PostCategoryId { get; set; }

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public virtual PostCategory PostCategory { get; set; } = null!;
}
