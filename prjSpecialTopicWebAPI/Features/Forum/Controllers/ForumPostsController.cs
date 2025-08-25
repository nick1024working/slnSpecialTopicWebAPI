using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Forum.Controllers
{
    [ApiController]
    [Route("api/forum/posts")]
    public class ForumPostsController : ControllerBase
    {
        private readonly TeamAProjectContext _db;
        public ForumPostsController(TeamAProjectContext db) => _db = db;

        // GET api/forum/posts/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _db.ForumPosts
                .Include(p => p.UidNavigation) // 若沒有導覽屬性就移除
                .FirstOrDefaultAsync(p => p.PostId == id && (p.IsDeleted != true));

            if (post == null) return NotFound();

            var images = await _db.PostImages
                .Where(i => i.PostId == id)
                .OrderBy(i => i.ImageId)
                .Select(i => i.PostImage1 != null
                    ? "data:image/png;base64," + Convert.ToBase64String(i.PostImage1)
                    : string.Empty)
                .ToListAsync();

            var vm = new
            {
                postId = post.PostId,
                title = post.Title,
                authorName = post.UidNavigation != null ? post.UidNavigation.Name : "匿名", // 這裡可以用 ?.
                createdAt = post.CreatedAt, // 若你的欄位不同請改
                viewCount = post.ViewCount,
                likeCount = post.LikeCount,
                contentHtml = post.GetType().GetProperty("ContentHtml") != null
                    ? (post.GetType().GetProperty("ContentHtml")!.GetValue(post)?.ToString() ?? "")
                    : (post.GetType().GetProperty("Content")?.GetValue(post)?.ToString() ?? ""),
                images
            };

            return Ok(vm);
        }

        // GET api/forum/posts/{id}/comments
        [HttpGet("{id:int}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var cs = await _db.PostComments
                .Where(c => c.PostId == id && (c.IsDeleted != true))
                .OrderBy(c => c.CreatedAt)
                .Select(c => new
                {
                    commentId = c.CommentId,
                    // ❌ c.UidNavigation?.Name ?? "匿名"  →  ✅ 三元運算子
                    authorName = c.UidNavigation != null ? c.UidNavigation.Name : "匿名",
                    createdAt = c.CreatedAt,
                    content = c.Content
                })
                .ToListAsync();

            return Ok(cs);
        }

        // GET api/forum/posts/by-category/{categoryId}
        [HttpGet("by-category/{categoryId:int}")]
        public async Task<IActionResult> GetPostsByCategory(int categoryId, int page = 1, int pageSize = 20)
        {
            var query = _db.ForumPosts
                .AsNoTracking()
                .Where(p => p.PostCategoryId == categoryId && (p.IsDeleted != true))
                .OrderByDescending(p => p.CreatedAt); // 改成你的時間欄位

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    postId = p.PostId,
                    title = p.Title,
                    authorName = p.UidNavigation != null ? p.UidNavigation.Name : "匿名",
                    createdAt = p.CreatedAt,
                    viewCount = p.ViewCount,
                    likeCount = p.LikeCount
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
