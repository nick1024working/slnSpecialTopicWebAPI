namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    public record ProjectListDto(
    int DonateProjectId,
    string ProjectTitle,
    string? ProjectDescription,
    decimal TargetAmount,
    decimal CurrentAmount,
    int BackerCount,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    string? MainImagePath,   // ← 由 donateImages.donateImagePath (is_main=1) 映射
    bool IsFavorite          // ← 由 donateProjects.projectIsFavorite 映射
);
}
