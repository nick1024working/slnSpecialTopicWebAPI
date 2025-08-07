namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public class UploadImagesRequest
    {
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
