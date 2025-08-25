namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
