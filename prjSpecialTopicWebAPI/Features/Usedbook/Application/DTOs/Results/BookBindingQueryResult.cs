using prjSpecialTopicWebAPI.Features.Usedbook.Application.Interfaces;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results
{
    public class BookBindingQueryResult : IHasIdName
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
