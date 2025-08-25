using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class PublicBookListItemQueryResult
    {
        public StorageProvider CoverStorageProvider { get; set; }
        public string CoverObjectKey { get; set; } = string.Empty;


        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public string ConditionRating { get; set; } = string.Empty;

        public IdNameDto Category { get; set; } = default!;
        public IReadOnlyList<IdNameDto> SaleTagList { get; set; } = [];

        public string Slug { get; set; } = string.Empty;
    }
}
