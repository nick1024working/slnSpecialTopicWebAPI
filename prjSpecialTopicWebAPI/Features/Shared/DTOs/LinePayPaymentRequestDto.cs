namespace prjSpecialTopicWebAPI.Features.Shared.DTOs
{
    public class LinePayPaymentRequestDto
    {
        public int Amount { get; set; }
        public string Currency { get; set; } = "TWD";
        public string OrderId { get; set; } = string.Empty;
        public List<PackageDto> Packages { get; set; } = [];
        public RedirectUrlsDto RedirectUrls { get; set; } = new RedirectUrlsDto();
    }

    public class PackageDto
    {
        public int Amount { get; set; }
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public List<ProductDto> Products { get; set; } = [];
    }

    public class ProductDto
    {
        public string? Id { get; set; }
        public string? ImageUrl { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? OriginalPrice { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }

    public class RedirectUrlsDto
    {
        public string ConfirmUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }
}
