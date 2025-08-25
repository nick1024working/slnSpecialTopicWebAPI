using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Ebook.DTOs;

using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/labels")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly TeamAProjectContext _db;

        public LabelsController(TeamAProjectContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 取得所有標籤列表
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLabels()
        {
            var labels = await _db.Labels
                .AsNoTracking()
                .OrderBy(l => l.LabelId)
                .Select(l => new LabelDto
                {
                    LabelId = l.LabelId,
                    LabelName = l.LabelName
                })
                .ToListAsync();
            return Ok(labels);
        }

        /// <summary>
        /// 新增一個標籤
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateLabel([FromBody] CreateLabelDto createDto)
        {
            var label = new Label
            {
                LabelName = createDto.LabelName,
            };
            _db.Labels.Add(label);
            await _db.SaveChangesAsync();

            var resultDto = new LabelDto
            {
                LabelId = label.LabelId,
                LabelName = label.LabelName
            };

            return CreatedAtAction(nameof(GetLabels), new { id = resultDto.LabelId }, resultDto);
        }
    }
}