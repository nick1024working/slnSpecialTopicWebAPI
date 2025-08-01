using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features._Demo
{
    [ApiController]
    [Route("api/demo")]
    public class AdminController : ControllerBase
    {
        private readonly TeamAProjectContext _db;

        public AdminController(TeamAProjectContext db)
        {
            _db = db;
        }

        [HttpGet("districts")]
        public IActionResult GetDistricts()
        {
            var dtoList = _db.Districts.ToList();
            return Ok(dtoList);
        }

        [HttpGet("districts/{id}")]
        public IActionResult GetDistrictById(int id)
        {
            var dto = _db.Districts.Where(d => d.Id == id).FirstOrDefault();
            return Ok(dto);
        }
    }
}
