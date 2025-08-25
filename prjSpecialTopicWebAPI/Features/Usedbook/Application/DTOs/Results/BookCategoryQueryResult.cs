using prjSpecialTopicWebAPI.Features.Usedbook.Application.Interfaces;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class BookCategoryQueryResult : IHasIdName
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;
    }
}
