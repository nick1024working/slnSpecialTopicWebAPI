namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    public record ProjectDetailDto : ProjectListDto
    {
        public string? LongDescription { get; set; }          // ← 由 donateProjects.projectLongDescription
        public IEnumerable<string>? Gallery { get; set; }     // ← 由 donateImages.projectGalleryPath / 非主圖
        public ProjectDetailDto(
            int donateProjectId, string projectTitle, string? projectDescription,
            decimal targetAmount, decimal currentAmount, int backerCount,
            DateTime startDate, DateTime endDate, string status, string? mainImagePath, bool isFavorite)
            : base(donateProjectId, projectTitle, projectDescription, targetAmount, currentAmount,
                   backerCount, startDate, endDate, status, mainImagePath, isFavorite)
        { }
    }
}
