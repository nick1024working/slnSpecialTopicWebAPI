using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace prjSpecialTopicWebAPI.Features.Forum.Controllers;

[ApiController]
[Route("api/forum/meta")]
public class PostMetaController : ControllerBase
{
    private readonly TeamAProjectContext _db;
    public PostMetaController(TeamAProjectContext db) { _db = db; }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var list = await _db.PostCategories
            .AsNoTracking()
            .Select(c => new { c.PostCategoryId, c.PostCategoryName })
            .OrderBy(c => c.PostCategoryName)
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters()
    {
        var list = await _db.PostFilters
            .AsNoTracking()
            .Select(f => new { f.PostFilterId, f.FilterName })
            .OrderBy(f => f.FilterName)
            .ToListAsync();
        return Ok(list);
    }
}
