using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Fund.Dtos
{
    public class CategoryCreateDto
    {
        [Required, StringLength(100)]
        public string CategoriesName { get; set; } = default!;
    }
}
