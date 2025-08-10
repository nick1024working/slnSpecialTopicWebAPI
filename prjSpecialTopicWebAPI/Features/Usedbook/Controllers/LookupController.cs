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

        [HttpGet("counties")]
        public async Task<ActionResult<IEnumerable<IdNameDto>>> GetCountyList(CancellationToken ct)
        {
            var queryResult = await _lookupService.GetCountyListAsync(ct);

            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);

            return Ok(queryResult.Value);
        }

        [HttpGet("counties/{countyId}/districts")]
        public async Task<ActionResult<IEnumerable<IdNameDto>>> GetDistrictListByCountyId([FromRoute] int countyId, CancellationToken ct)
        {
            var queryResult = await _lookupService.GetDistrictListByCountyIdAsync(countyId, ct);

            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);

            return Ok(queryResult.Value);
        }

        [HttpGet("languages")]
        public async Task<ActionResult<IEnumerable<IdNameDto>>> GetLanguageList(CancellationToken ct)
        {
            var queryResult = await _lookupService.GetLanguageListAsync(ct);

            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);

            return Ok(queryResult.Value);
        }

        [HttpGet("usedbooks/condition-rating-desc/{id:int}")]
        public async Task<ActionResult<BookConditionRatingDescriptionDto>> GetBookConditionRatingDescriptionById([FromRoute] int id, CancellationToken ct)
        {
            var queryResult = await _lookupService.GetBookConditionRatingDescriptionById(id, ct);

            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);

            return Ok(queryResult.Value);
        }

        [HttpGet("usedbooks/all-ui-lookups")]
        public async Task<ActionResult<AllUsedBookLookupListsDto>> GetAllUsedBookUILookupsList( CancellationToken ct)
        {
            var queryResult = await _lookupService.GetAllUsedBookUILookupsList(ct);

            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);

            return Ok(queryResult.Value);
        }
    }
}
