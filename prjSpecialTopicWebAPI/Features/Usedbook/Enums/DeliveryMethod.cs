using prjSpecialTopicWebAPI.Features.Shared.Enums;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Enums
{
    public enum DeliveryMethod : byte
    {
        FaceToFace = 0,
        HomeDeliveryHCT = 1,
        C711PickupPay = 2,
        C711PickupOnly = 3,
    }

    public static class DeliveryMapper
    {
        public static readonly Dictionary<DeliveryMethod, decimal> ToFee = new()
        {
            { DeliveryMethod.FaceToFace, 0m },
            { DeliveryMethod.HomeDeliveryHCT, 120m },
            { DeliveryMethod.C711PickupPay, 60m },
            { DeliveryMethod.C711PickupOnly, 60m },
        };
    }
}
