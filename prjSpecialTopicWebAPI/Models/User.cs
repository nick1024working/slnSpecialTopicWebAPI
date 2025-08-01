using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class User
{
    public Guid Uid { get; set; }

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool Gender { get; set; }

    public DateOnly Birthday { get; set; }

    public string? Address { get; set; }

    public DateTime? RegisterDate { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public byte? Status { get; set; }

    public byte? Level { get; set; }

    public string? AvatarUrl { get; set; }

    public bool? IsAuthor { get; set; }

    public byte? AuthorStatus { get; set; }

    public virtual ICollection<EBookOrderMain> EBookOrderMains { get; set; } = new List<EBookOrderMain>();

    public virtual ICollection<EbookPurchased> EbookPurchaseds { get; set; } = new List<EbookPurchased>();

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public virtual ICollection<LoginLog> LoginLogs { get; set; } = new List<LoginLog>();

    public virtual ICollection<PostBookmark> PostBookmarks { get; set; } = new List<PostBookmark>();

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual ICollection<Subscriber> Subscribers { get; set; } = new List<Subscriber>();

    public virtual ICollection<UsedBook> UsedBooks { get; set; } = new List<UsedBook>();
}
