using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Controllers
{
    [ApiController]
    [Route("api/lookup")]
    public class LookupController : ControllerBase
    {
        private readonly LookupService _lookupService;

        public LookupController(
            LookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet("districts")]
        public async Task<ActionResult<IEnumerable<IdNameDto>>> GetDistrictListByCountyId([FromQuery] int countyId, CancellationToken ct)
        {
            var queryResult = await _lookupService.GetDistrictListByCountyIdAsync(countyId, ct);

            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);

            return Ok(queryResult);
        }

        [HttpGet("bookConditionRatingDescription")]
        public async Task<ActionResult<string>> GetBookConditionRatingDescriptionById([FromQuery] int id, CancellationToken ct)
        {
            var queryResult = await _lookupService.GetBookConditionRatingDescriptionById(id, ct);

            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);

            return Ok(queryResult);
        }
    }
}
