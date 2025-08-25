using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Dtos;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Forum.Controllers
{// Features/Forum/Controllers/ForumCategoryController.cs
    [ApiController]
    [Route("api/forum/categories")]
    public class ForumCategoryController : ControllerBase
    {
        private readonly TeamAProjectContext _context;
        public ForumCategoryController(TeamAProjectContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumCategory>>> GetCategories()
        {
            var categories = await _context.PostCategories
                .AsNoTracking()
                .Select(c => new ForumCategory
                {
                    CategoryId = c.PostCategoryId,
                    Name = c.PostCategoryName,
                    PostCount = c.ForumPosts.Count(p => !p.IsDeleted),
                    // TODO: 把 CreatedAt 改成你 ForumPosts 的實際「發文/建立時間」欄位
                    LastPostAt = c.ForumPosts.Where(p => !p.IsDeleted)
                                             .Max(p => (DateTime?)p.CreatedAt),
                    // TODO: 使用者名稱欄位請對齊你的模型（例如 p.UidNavigation.Name）
                    LastPoster = c.ForumPosts.Where(p => !p.IsDeleted)
                                             .OrderByDescending(p => p.CreatedAt)
                                             .Select(p => p.UidNavigation.Name)
                                             .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(categories);
        }
    }

}
