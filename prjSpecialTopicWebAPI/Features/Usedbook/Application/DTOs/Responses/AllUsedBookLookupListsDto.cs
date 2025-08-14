using Microsoft.AspNetCore.Mvc.Rendering;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class AllUsedBookLookupListsDto
    {
        public IEnumerable<IdNameDto> BookBindings { get; set; } = [];
        public IEnumerable<IdNameDto> BookConditionRatings { get; set; } = [];
        public IEnumerable<IdNameDto> ContentRatings { get; set; } = [];
        public IEnumerable<IdNameDto> Counties { get; set; } = [];
        public IEnumerable<IdNameDto> Languages { get; set; } = [];
    }
}
