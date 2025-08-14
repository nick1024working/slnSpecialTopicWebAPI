using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class CreateLabelDto
    {
        [Required]
        [MaxLength(50)]
        public string LabelName { get; set; } = null!;
    }
}
