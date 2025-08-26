namespace prjSpecialTopicWebAPI.Features.Usedbook.Enums
{
    public enum PaymentStatus : byte
    {
        Unpaid = 0,
        Failed = 1,
        Expired = 2,
        Paid = 3,
        Refunding = 4,
        Refunded = 5,
    }
}
