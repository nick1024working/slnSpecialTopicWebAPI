// prjSpecialTopicWebAPI/Dtos/ForumCategory.cs
namespace prjSpecialTopicWebAPI.Dtos;

public class ForumCategory
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = "";
    public int PostCount { get; set; }
    public DateTime? LastPostAt { get; set; }
    public string? LastPoster { get; set; }
}