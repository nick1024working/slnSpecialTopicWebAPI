using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class PostCategory
{
    public int PostCategoryId { get; set; }

    public string PostCategoryName { get; set; } = null!;

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public virtual ICollection<PostFilter> PostFilters { get; set; } = new List<PostFilter>();
}
