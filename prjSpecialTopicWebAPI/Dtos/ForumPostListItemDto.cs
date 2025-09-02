// prjSpecialTopicWebAPI/Dtos/ForumPostListItemDto.cs
namespace prjSpecialTopicWebAPI.Dtos
{
    public class ForumPostListItemDto
    {
        public int PostId { get; set; }
        public string Title { get; set; } = "";
        public string AuthorName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }  // 若沒有可維持 0
        public string? Excerpt { get; set; } // 若沒有可留空
    }
}
