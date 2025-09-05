// prjSpecialTopicWebAPI/Dtos/PagedResultDto.cs
namespace prjSpecialTopicWebAPI.Dtos
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
    }
}
