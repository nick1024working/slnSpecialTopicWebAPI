using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public record UpdateBookCategoryRequest
    {
        public List<Guid> BookIdList { get; set; } = [];

        public int CategoryId { get; set; }
    }
}
