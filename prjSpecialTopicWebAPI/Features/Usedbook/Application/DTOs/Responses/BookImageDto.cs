using prjSpecialTopicWebAPI.Features.Usedbook.Enums;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class BookImageDto
    {
        public int Id { get; set; }
        public bool IsCover { get; set; }
        public byte ImageIndex { get; set; }
        public StorageProvider StorageProvider { get; set; }
        public string ObjectKey { get; set; } = string.Empty;
        //public byte[] Sha256 { get; set; } = null!;
    }
}