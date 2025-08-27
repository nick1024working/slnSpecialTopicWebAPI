namespace prjSpecialTopicWebAPI.Features.Shared.Enums
{
    public enum DeliveryOption
    {
        NoDelivery = 0,
        HomeDeliveryHCT = 1,
        C711PickupPay = 2,
        C711PickupOnly = 3,
        FaceToFace = 4,
    }

    public static class DeliveryMapper
    {
        private static readonly Dictionary<DeliveryOption, decimal> ToFee = new()
        {
            { DeliveryOption.NoDelivery, 0m },
            { DeliveryOption.HomeDeliveryHCT, 120m },
            { DeliveryOption.C711PickupPay, 60m },
            { DeliveryOption.C711PickupOnly, 60m },
            { DeliveryOption.FaceToFace, 0m },
        };
    }
}