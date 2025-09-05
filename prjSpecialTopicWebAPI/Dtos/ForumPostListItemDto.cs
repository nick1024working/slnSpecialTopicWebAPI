// prjSpecialTopicWebAPI/Dtos/ForumPostListItemDto.cs
namespace prjSpecialTopicWebAPI.Dtos
{
    public record PostDetailDto(
        int PostId, string? Title, string AuthorName, DateTime? CreatedAt,
        int? ViewCount, int? LikeCount, string ContentHtml, IReadOnlyList<string> Images,
        int BoardId, string BoardName,
        bool LikedByMe,
        int PostCategoryID // 新增：讓前端可直接用 postCategoryID
    );
}
