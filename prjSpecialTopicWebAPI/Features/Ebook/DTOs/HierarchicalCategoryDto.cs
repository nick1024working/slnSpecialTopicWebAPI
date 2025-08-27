namespace prjSpecialTopicWebAPI.Features.Ebook.DTOs
{
    // 這個 DTO 代表下拉選單中的一個選項
    public class CategoryOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    // 這個 DTO 代表一個父分類，它包含自己的資訊和一個子分類列表
    public class HierarchicalCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<CategoryOptionDto> Children { get; set; } = new();
    }
}
