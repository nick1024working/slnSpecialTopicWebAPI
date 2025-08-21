using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models; // ← 你的命名空間
using System.Linq;

namespace prjSpecialTopicWebAPI.Features.Forum.Controllers
{
    [ApiController]
    [Route("api/forum/posts")]
    public class ForumPostsController : ControllerBase
    {
        private readonly TeamAProjectContext _db;
        public ForumPostsController(TeamAProjectContext db) => _db = db;

        // GET /api/forum/posts/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPost(int id)
        {
            // 先拿文章（不用 Include，避免你目前沒有導覽屬性）
            var post = await _db.ForumPosts
                .Include(p => p.UidNavigation)  // 有作者關聯就保留，沒有可拿掉
                .FirstOrDefaultAsync(p => p.PostId == id && (p.IsDeleted != true));
            if (post == null) return NotFound();

            // 另外查圖片清單
            var images = await _db.PostImages
                .Where(i => i.PostId == id)
                .OrderBy(i => i.ImageId)   // ← 排序就用 ImageId
                .Select(i => "data:image/png;base64," + Convert.ToBase64String(i.PostImage1))
                .ToListAsync();

            var vm = new
            {
                postId = post.PostId,
                title = post.Title,
                authorName = post.UidNavigation?.Name ?? "匿名",
                createdAt = post.CreatedAt,
                viewCount = post.ViewCount,
                likeCount = post.LikeCount,
                // ⭐ 同時相容 ContentHtml/Content
                contentHtml = post.GetType().GetProperty("ContentHtml") != null
                    ? (post.GetType().GetProperty("ContentHtml")!.GetValue(post)?.ToString() ?? "")
                    : (post.GetType().GetProperty("Content")?.GetValue(post)?.ToString() ?? ""),
                images
            };
            return Ok(vm);
        }

        // GET /api/forum/posts/{id}/comments
        [HttpGet("{id:int}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var cs = await _db.PostComments
                .Where(c => c.PostId == id && (c.IsDeleted != true)) // ⭐ bool? 相容
                .OrderBy(c => c.CreatedAt)
                .Select(c => new
                {
                    commentId = c.CommentId,
                    authorName = c.UidNavigation!.Name, // 若可能為 null 改成 c.UidNavigation?.Name ?? "匿名"
                    createdAt = c.CreatedAt,
                    content = c.Content
                })
                .ToListAsync();

            return Ok(cs);
        }

        // GET /api/forum/categories
        [HttpGet("/api/forum/categories")]
        public async Task<IActionResult> GetCategories()
        {
            // ⭐ DbSet 用複數
            var list = await _db.PostCategories
                .OrderBy(c => c.PostCategoryId) // 依你的欄位改：PostCategoryID / PostCategoryId
                .Select(c => new
                {
                    id = c.PostCategoryId,       // 或 c.PostCategoryID
                    name = c.PostCategoryName
                })
                .ToListAsync();

            return Ok(list);
        }

        private string ToUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "";
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return path;
            if (!path.StartsWith("/")) path = "/" + path;
            var req = HttpContext.Request;
            var baseUrl = $"{req.Scheme}://{req.Host}";
            return baseUrl + path; // e.g. https://localhost:7104/uploads/xxx.jpg
        }
    }
}
