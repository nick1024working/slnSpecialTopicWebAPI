namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public sealed class PatchItemRequest
    {
        public CartItemDto Item { get; set; } = default!;
    }
}
