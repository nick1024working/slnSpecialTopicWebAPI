namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int? ParentCategoryId { get; set; }
    }
}
