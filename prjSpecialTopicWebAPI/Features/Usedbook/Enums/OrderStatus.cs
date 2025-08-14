namespace prjSpecialTopicWebAPI.Features.Usedbook.Enums
{
    public enum OrderStatus : byte
    {
        Pending = 0,
        Paid = 1,
        Processing = 2,
        Completed = 3,
        Cancelled = 4,
        Disputed = 5
    }
}
