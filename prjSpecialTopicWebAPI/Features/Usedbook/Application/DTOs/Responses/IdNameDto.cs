using prjSpecialTopicWebAPI.Features.Usedbook.Application.Interfaces;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses
{
    public class IdNameDto : IHasIdName
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
