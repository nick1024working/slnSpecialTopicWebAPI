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
                .Include(p => p.UidNavigation)
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
                authorName = post.UidNavigation != null ? post.UidNavigation.Name : "匿名",
                createdAt = post.CreatedAt,
                viewCount = post.ViewCount,
                likeCount = post.LikeCount,
                // 你的模型沒有 ContentHtml，就用 Content；避免 Null
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
                    authorName = c.UidNavigation != null ? c.UidNavigation.Name : "匿名",
                    createdAt = c.CreatedAt,
                    content = c.Content
                })
                .ToListAsync();

            return Ok(cs);
        }

        // GET api/forum/posts
        [HttpGet]
        public async Task<IActionResult> GetPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? boardId = null,
            [FromQuery] string orderBy = "new")
        {
            // 清單查詢建議 AsNoTracking 提升效能
            var q = _db.ForumPosts
                .AsNoTracking()
                .Include(p => p.UidNavigation)
                .Include(p => p.PostCategory)
                .Where(p => p.IsDeleted != true);

            if (boardId.HasValue)
                q = q.Where(p => p.PostCategoryId == boardId.Value);

            // 先投影成匿名型別，避免在記憶體中二次處理
            var query = q.Select(p => new
            {
                PostId = p.PostId,
                Title = p.Title,
                BoardId = p.PostCategoryId,
                BoardName = p.PostCategory.PostCategoryName,
                AuthorName = p.UidNavigation != null ? p.UidNavigation.Name : "匿名",
                CreatedAt = p.CreatedAt,
                ViewCount = p.ViewCount,
                // 不需要 Include PostLikes，Count 會翻成 SQL
                LikeCount = p.PostLikes.Count(),
                ReplyCount = p.PostComments.Count(c => c.IsDeleted != true),
                Excerpt = ((p.Content ?? "").Length > 100)
                               ? (p.Content ?? "").Substring(0, 100) + "..."
                               : (p.Content ?? "")
            });

            query = orderBy == "hot"
                ? query.OrderByDescending(p => p.ViewCount + (p.LikeCount * 3))
                : query.OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }

        // GET api/forum/posts/by-category/{categoryId}
        [HttpGet("by-category/{categoryId:int}")]
        public async Task<IActionResult> GetPostsByCategory(
            int categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (categoryId <= 0)
                return BadRequest("Category ID must be a positive integer.");

            var query = _db.ForumPosts
                .AsNoTracking()
                .Include(p => p.UidNavigation)
                .Include(p => p.PostCategory)
                .Where(p => p.PostCategoryId == categoryId && (p.IsDeleted != true))
                .OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    postId = p.PostId,
                    title = p.Title,
                    authorName = p.UidNavigation != null ? p.UidNavigation.Name : "匿名",
                    createdAt = p.CreatedAt,
                    viewCount = p.ViewCount,
                    likeCount = p.PostLikes.Count(),
                    replyCount = p.PostComments.Count(c => c.IsDeleted != true),
                    excerpt = ((p.Content ?? "").Length > 100)
                                   ? (p.Content ?? "").Substring(0, 100) + "..."
                                   : (p.Content ?? "")
                })
                .ToListAsync();

            return Ok(new
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }
    }
}
