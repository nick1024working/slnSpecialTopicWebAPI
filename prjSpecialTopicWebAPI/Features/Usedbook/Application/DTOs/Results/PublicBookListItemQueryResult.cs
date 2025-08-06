using prjSpecialTopicWebAPI.Features.Usedbook.Enums;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class PublicBookListItemQueryResult
    {
        public StorageProvider CoverStorageProvider { get; set; }
        public string CoverObjectKey { get; set; } = string.Empty;

        public IReadOnlyList<string> SaleTagList { get; set; } = [];

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public string ConditionRating { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;
    }
}
