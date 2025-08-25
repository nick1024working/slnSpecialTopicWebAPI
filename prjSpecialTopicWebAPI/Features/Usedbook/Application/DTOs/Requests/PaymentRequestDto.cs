namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public class PaymentRequestDto
    {
        public int Amount { get; set; }
        public string Currency { get; set; } = "TWD";
        public string OrderId { get; set; } = string.Empty;
        public IEnumerable<PackageDto> Packages { get; set; } = [];
        public RedirectUrlsDto RedirectUrls { get; set; } = new RedirectUrlsDto();
    }

    public class PackageDto
    {
        public int Amount { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public IEnumerable<ProductDto> Products { get; set; } = [];
        public int? UserFee { get; set; }
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
        public string ConfirmUrl { get; set; } = "https://pay-store.example.com/order/payment/authorize";
        public string CancelUrl { get; set; } = "https://pay-store.example.com/order/payment/cancel";
    }
}
