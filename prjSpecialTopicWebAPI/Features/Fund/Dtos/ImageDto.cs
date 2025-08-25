namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    public record ImageDto(
       int DonateImageId,
       int DonateProjectId,
       string? DonateImagePath,     // 主圖路徑
       bool? IsMain,                // 只有主圖為 true
       string? ProjectGalleryPath   // 相簿路徑
   );
}
