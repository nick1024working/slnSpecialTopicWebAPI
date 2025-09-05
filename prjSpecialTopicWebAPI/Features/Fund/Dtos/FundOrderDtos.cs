namespace prjSpecialTopicWebAPI.Features.Fund.Dtos;

public record CreateOrderDto(
    decimal TotalAmount,
    string? PaymentMethod,
    int? ProjectId = null,
    int? DonatePlanId = null,
    int Quantity = 1
);

public record OrderDto(
    int DonateOrderId,
    decimal TotalAmount,
    string? PaymentMethod,
    DateTime? PaymentDate,
    DateTime OrderCreatedAt,
    int? DonateProjectId,
    int? DonatePlanId,
    string? ProjectTitle = null,
    string? PlanTitle = null
);

public record PayDto(
    string PaymentMethod
);
