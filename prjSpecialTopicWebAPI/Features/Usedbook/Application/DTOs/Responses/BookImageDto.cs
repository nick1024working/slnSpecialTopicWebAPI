namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class BookImageDto
    {
        public int Id { get; set; }
        public bool IsCover { get; set; }
        public int DisplayOrder { get; set; }

        public string MainbUrl { get; set; } = string.Empty;
        public string ThumbUrl { get; set; } = string.Empty;
        //public StorageProvider StorageProvider { get; set; }
        //public string ObjectKey { get; set; } = string.Empty;

        //public byte[] Sha256 { get; set; } = null!;
    }
}