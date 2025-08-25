namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    public class ProjectCreateDto
    {
        public int DonateCategoriesId { get; set; }
        public Guid UID { get; set; }
        public string ProjectTitle { get; set; } = default!;
        public string? ProjectDescription { get; set; }        // text 可為 null
        public decimal TargetAmount { get; set; }
        public DateTime StartDate { get; set; }                // DB 是 date，用 DateTime 也可
        public DateTime EndDate { get; set; }
        public string? LongDescription { get; set; }           // optional，對應 projectLongDescription
        public bool? IsFavorite { get; set; }                  // optional，對應 projectIsFavorite
        public string? MainImagePath { get; set; }             // optional，若有同時幫你建一筆 is_main=1
        public IEnumerable<string>? Gallery { get; set; }      // optional，projectGalleryPath 多筆
    }
}
