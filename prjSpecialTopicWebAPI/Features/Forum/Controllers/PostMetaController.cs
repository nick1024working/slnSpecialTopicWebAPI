using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/forum")]
public class ForumMetaController : ControllerBase
{
    private readonly TeamAProjectContext _db;
    public ForumMetaController(TeamAProjectContext db) => _db = db;

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var items = await _db.PostCategories
            .AsNoTracking()
            .OrderBy(c => c.PostCategoryId)
            .Select(c => new { id = c.PostCategoryId, name = c.PostCategoryName })
            .ToListAsync();
        return Ok(items);
    }
}

