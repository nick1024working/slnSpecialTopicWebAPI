using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;
using System.Text.Json.Serialization;
using System.Security.Claims;

namespace prjSpecialTopicWebAPI.Features.Forum.Controllers
{
    [ApiController]
    [Route("api/forum/posts")]
    public class ForumPostsController : ControllerBase
    {
        private readonly TeamAProjectContext _db;
        public ForumPostsController(TeamAProjectContext db) => _db = db;

        // ========== 共用 DTO ==========
        public record PagedResult<T>(
            [property: JsonPropertyName("items")] IReadOnlyList<T> Items,
            [property: JsonPropertyName("page")] int Page,
            [property: JsonPropertyName("pageSize")] int PageSize,
            [property: JsonPropertyName("total")] int Total,
            [property: JsonPropertyName("totalPages")] int TotalPages
        );

        public record PostListItemDto(
            [property: JsonPropertyName("postId")] int PostId,
            [property: JsonPropertyName("title")] string? Title,
            [property: JsonPropertyName("boardId")] int BoardId,
            [property: JsonPropertyName("boardName")] string BoardName,
            [property: JsonPropertyName("authorName")] string AuthorName,
            [property: JsonPropertyName("createdAt")] DateTime? CreatedAt,
            [property: JsonPropertyName("viewCount")] int? ViewCount,
            [property: JsonPropertyName("likeCount")] int LikeCount,
            [property: JsonPropertyName("replyCount")] int ReplyCount,
            [property: JsonPropertyName("excerpt")] string Excerpt
        );

        public record PostDetailDto(
            int PostId, string? Title, string AuthorName, DateTime? CreatedAt,
            int? ViewCount, int? LikeCount, string ContentHtml, IReadOnlyList<string> Images,
            int BoardId, string BoardName,
            bool LikedByMe // ★ 新增
        );

        public record CommentDto(
            [property: JsonPropertyName("commentId")] int CommentId,
            [property: JsonPropertyName("authorName")] string AuthorName,
            [property: JsonPropertyName("createdAt")] DateTime? CreatedAt,
            [property: JsonPropertyName("content")] string? Content
        );

        // ========== 小工具 ==========
        private static (int page, int pageSize) NormalizePaging(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            return (page, pageSize);
        }

        private static string ToDataUrl(byte[]? bytes)
            => bytes is { Length: > 0 }
               ? $"data:image/png;base64,{Convert.ToBase64String(bytes)}"
               : string.Empty;

        // ========== 單篇 ==========
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPost(int id)
        {
            // 1) 原子性把 ViewCount + 1
            var rows = await _db.ForumPosts
                .Where(p => p.PostId == id && p.IsDeleted != true)
                .ExecuteUpdateAsync(up => up.SetProperty(
                    p => p.ViewCount,
                    p => p.ViewCount + 1
                ));

            if (rows == 0) return NotFound();

            // 2) 再讀一次最新資料（含分類、作者）
            var post = await _db.ForumPosts
                .AsNoTracking()
                .Include(p => p.UidNavigation)
                .Include(p => p.PostCategory) // ★ 加入分類
                .FirstOrDefaultAsync(p => p.PostId == id);

            var images = await _db.PostImages
                .AsNoTracking()
                .Where(i => i.PostId == id)
                .OrderByDescending(i => i.IsMainPic)
                .ThenBy(i => i.ImageId)
                .Select(i => ToDataUrl(i.PostImage1))
                .ToListAsync();
            Guid uid;
            var uidStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!string.IsNullOrWhiteSpace(uidStr) && Guid.TryParse(uidStr, out var claimUid))
                uid = claimUid;
            else
                uid = await _db.Users.AsNoTracking().Select(u => u.Uid).FirstOrDefaultAsync();

            // 用 PostLikes 計數 + 判斷是否已按過
            var likeCount = await _db.PostLikes.CountAsync(x => x.PostId == id);
            var likedByMe = uid != Guid.Empty &&
                            await _db.PostLikes.AnyAsync(x => x.PostId == id && x.Uid == uid);

            var dto = new PostDetailDto(
                PostId: post!.PostId,
                Title: post.Title,
                AuthorName: post.UidNavigation != null ? post.UidNavigation.Name : "匿名",
                CreatedAt: post.CreatedAt,
                ViewCount: post.ViewCount,
                LikeCount: likeCount,
                ContentHtml: post.Content ?? string.Empty,
                Images: images,
                BoardId: post.PostCategoryId,
                BoardName: post.PostCategory?.PostCategoryName ?? string.Empty,
                LikedByMe: likedByMe
            );

            return Ok(dto);
        }

        // ========== 留言 ==========
        [HttpGet("{id:int}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var comments = await _db.PostComments
                .AsNoTracking()
                .Where(c => c.PostId == id && (c.IsDeleted != true))
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto(
                    c.CommentId,
                    c.UidNavigation != null ? c.UidNavigation.Name : "匿名",
                    c.CreatedAt,
                    c.Content
                ))
                .ToListAsync();

            return Ok(comments);
        }

        // ★ 新增：建立留言
        public record CreateCommentRequest(string? Content);

        [HttpPost("{id:int}/comments")]
        public async Task<IActionResult> CreateComment(int id, [FromBody] CreateCommentRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Content)) return BadRequest("留言內容不可為空");
            var post = await _db.ForumPosts.FirstOrDefaultAsync(p => p.PostId == id && p.IsDeleted != true);
            if (post == null) return NotFound("找不到文章");

            // 取得使用者 UID（claims → fallback 首位使用者）
            Guid uid;
            var uidStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!string.IsNullOrWhiteSpace(uidStr) && Guid.TryParse(uidStr, out var claimUid))
                uid = claimUid;
            else
            {
                var firstUser = await _db.Users.AsNoTracking().Select(u => u.Uid).FirstOrDefaultAsync();
                if (firstUser == Guid.Empty) return Unauthorized("尚未登入，且系統內沒有可用的預設使用者。");
                uid = firstUser;
            }

            var c = new PostComment
            {
                PostId = id,
                Uid = uid,
                Content = req.Content!.Trim(),
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            _db.PostComments.Add(c);
            await _db.SaveChangesAsync();

            // 回傳建立好的留言（給前端覆蓋暫存留言）
            var authorName = await _db.Users.AsNoTracking()
                .Where(u => u.Uid == uid).Select(u => u.Name).FirstOrDefaultAsync() ?? "匿名";

            var dto = new CommentDto(
                CommentId: c.CommentId,
                AuthorName: authorName,
                CreatedAt: c.CreatedAt,
                Content: c.Content
            );
            return CreatedAtAction(nameof(GetComments), new { id }, dto);
        }

        // ========== 清單（略，同你原本） ==========
        [HttpGet]
        public async Task<IActionResult> GetPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? boardId = null,
            [FromQuery] string orderBy = "new")
        {
            (page, pageSize) = NormalizePaging(page, pageSize);

            var baseQ = _db.ForumPosts
                .AsNoTracking()
                .Where(p => p.IsDeleted != true);

            if (boardId.HasValue)
                baseQ = baseQ.Where(p => p.PostCategoryId == boardId.Value);

            var q = baseQ.Select(p => new
            {
                p.PostId,
                p.Title,
                p.PostCategoryId,
                BoardName = p.PostCategory.PostCategoryName,
                AuthorName = p.UidNavigation != null ? p.UidNavigation.Name : "匿名",
                p.CreatedAt,
                ViewCount = p.ViewCount,
                LikeCount = p.PostLikes.Count(),
                ReplyCount = p.PostComments.Count(c => c.IsDeleted != true),
                Excerpt = (p.Content ?? ""),
                HotScore = p.ViewCount + p.PostLikes.Count() * 3
            });

            string key = (orderBy ?? "new").Trim().ToLowerInvariant();
            switch (key)
            {
                case "hot":
                case "熱門":
                    q = q.OrderByDescending(p => p.HotScore).ThenByDescending(p => p.CreatedAt);
                    break;
                case "view":
                case "views":
                case "瀏覽":
                    q = baseQ
                        .OrderByDescending(p => p.ViewCount)
                        .ThenByDescending(p => p.CreatedAt)
                        .Select(p => new {
                            p.PostId,
                            p.Title,
                            p.PostCategoryId,
                            BoardName = p.PostCategory.PostCategoryName,
                            AuthorName = p.UidNavigation != null ? p.UidNavigation.Name : "匿名",
                            p.CreatedAt,
                            ViewCount = p.ViewCount,
                            LikeCount = p.PostLikes.Count(),
                            ReplyCount = p.PostComments.Count(c => c.IsDeleted != true),
                            Excerpt = (p.Content ?? ""),
                            HotScore = p.ViewCount + p.PostLikes.Count() * 3
                        });
                    break;
                case "like":
                case "likes":
                case "喜歡":
                    q = q.OrderByDescending(p => p.LikeCount).ThenByDescending(p => p.CreatedAt);
                    break;
                case "reply":
                case "replies":
                case "回覆":
                    q = q.OrderByDescending(p => p.ReplyCount).ThenByDescending(p => p.CreatedAt);
                    break;
                case "new":
                case "最新":
                default:
                    q = q.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            var total = await q.CountAsync();

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostListItemDto(
                    p.PostId,
                    p.Title,
                    p.PostCategoryId,
                    p.BoardName,
                    p.AuthorName,
                    p.CreatedAt,
                    p.ViewCount,
                    p.LikeCount,
                    p.ReplyCount,
                    p.Excerpt.Length > 100 ? p.Excerpt.Substring(0, 100) + "..." : p.Excerpt
                ))
                .ToListAsync();

            return Ok(new PagedResult<PostListItemDto>(
                Items: items,
                Page: page,
                PageSize: pageSize,
                Total: total,
                TotalPages: (int)Math.Ceiling(total / (double)pageSize)
            ));
        }

        // ========== 文章更新/刪除/建立（沿用你原本） ==========
        public record UpdatePostRequest(string? Title, string? ContentHtml, int? PostCategoryID);

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest req)
        {
            var post = await _db.ForumPosts.FirstOrDefaultAsync(p => p.PostId == id && p.IsDeleted != true);
            if (post == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(req.Title)) post.Title = req.Title!.Trim();
            if (req.ContentHtml is not null) post.Content = req.ContentHtml;
            if (req.PostCategoryID.HasValue && req.PostCategoryID.Value > 0)
            {
                var ok = await _db.PostCategories.AnyAsync(c => c.PostCategoryId == req.PostCategoryID.Value);
                if (!ok) return BadRequest($"分類不存在：{req.PostCategoryID.Value}");
                post.PostCategoryId = req.PostCategoryID.Value;
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _db.ForumPosts.FirstOrDefaultAsync(p => p.PostId == id && p.IsDeleted != true);
            if (post == null) return NotFound();

            post.IsDeleted = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
        // using 加上：using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
        [HttpPost("{id:int}/like")]
        public async Task<IActionResult> LikePost(int id)
        {
            var postExists = await _db.ForumPosts.AnyAsync(p => p.PostId == id && !p.IsDeleted);
            if (!postExists) return NotFound(new { message = "Post not found." });

            // 取得使用者
            Guid uid;
            var uidStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!string.IsNullOrWhiteSpace(uidStr) && Guid.TryParse(uidStr, out var claimUid))
                uid = claimUid;
            else
                uid = await _db.Users.AsNoTracking().Select(u => u.Uid).FirstOrDefaultAsync();
            if (uid == Guid.Empty) return Unauthorized("尚未登入。");

            // 已按讚就不要重覆
            var exists = await _db.PostLikes.AnyAsync(x => x.PostId == id && x.Uid == uid);
            if (!exists)
            {
                _db.PostLikes.Add(new PostLike { PostId = id, Uid = uid, });
                await _db.SaveChangesAsync();
            }

            var likeCount = await _db.PostLikes.CountAsync(x => x.PostId == id);
            return Ok(new { liked = true, likeCount });
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> CreatePost()
        {
            try
            {
                var title = Request.Form["title"].ToString()?.Trim();
                var contentHtml = Request.Form["contentHtml"].ToString();
                var postCategoryIDStr = Request.Form["postCategoryID"].ToString();
                var mainIndexStr = Request.Form["mainIndex"].ToString();

                if (string.IsNullOrWhiteSpace(title))
                    return BadRequest("標題為必填。");
                if (!int.TryParse(postCategoryIDStr, out var postCategoryId) || postCategoryId <= 0)
                    return BadRequest("postCategoryID 無效。");

                var categoryExists = await _db.PostCategories
                    .AsNoTracking()
                    .AnyAsync(c => c.PostCategoryId == postCategoryId);
                if (!categoryExists)
                    return BadRequest($"找不到 PostCategoryId={postCategoryId}。");

                int? mainIndex = null;
                if (int.TryParse(mainIndexStr, out var mix) && mix >= 0)
                    mainIndex = mix;

                Guid uid;
                var uidStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue("sub");
                if (!string.IsNullOrWhiteSpace(uidStr) && Guid.TryParse(uidStr, out var claimUid))
                    uid = claimUid;
                else
                {
                    var firstUser = await _db.Users
                        .AsNoTracking()
                        .Select(u => u.Uid)
                        .FirstOrDefaultAsync();

                    if (firstUser == Guid.Empty)
                        return Unauthorized("尚未登入，且系統內沒有可用的預設使用者。");
                    uid = firstUser;
                }

                var defaultFilter = await _db.PostFilters
                    .OrderBy(f => f.PostFilterId)
                    .FirstOrDefaultAsync();

                if (defaultFilter == null)
                    return Problem(title: "建立文章失敗",
                                   detail: "系統尚未建立任何 PostFilter，請先建立至少一筆。",
                                   statusCode: 500);

                var post = new ForumPost
                {
                    Title = title!,
                    Content = contentHtml,
                    PostCategoryId = postCategoryId,
                    Uid = uid,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                    ViewCount = 0,
                    LikeCount = 0,
                    Filter = defaultFilter
                };

                _db.ForumPosts.Add(post);
                await _db.SaveChangesAsync();

                var files = Request.Form.Files;
                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var f = files[i];
                        if (f.Length <= 0) continue;

                        using var ms = new MemoryStream();
                        await f.CopyToAsync(ms);
                        var bytes = ms.ToArray();

                        _db.PostImages.Add(new PostImage
                        {
                            PostId = post.PostId,
                            PostImage1 = bytes,
                            IsMainPic = (mainIndex.HasValue && mainIndex.Value == i),
                        });
                    }
                    await _db.SaveChangesAsync();

                    if (mainIndex.HasValue && mainIndex.Value >= files.Count)
                    {
                        var imgs = await _db.PostImages.Where(x => x.PostId == post.PostId).ToListAsync();
                        foreach (var im in imgs) im.IsMainPic = false;
                        await _db.SaveChangesAsync();
                    }
                }

                return CreatedAtAction(nameof(GetPost), new { id = post.PostId }, new { postId = post.PostId });
            }
            catch (DbUpdateException ex)
            {
                return Problem(title: "資料庫寫入失敗", detail: ex.InnerException?.Message ?? ex.Message, statusCode: 500);
            }
            catch (Exception ex)
            {
                return Problem(title: "建立文章發生未預期錯誤", detail: ex.Message, statusCode: 500);
            }
        }
    }
}
