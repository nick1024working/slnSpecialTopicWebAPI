namespace prjSpecialTopicWebAPI.Features.Usedbook.Utilities
{
    public static class OrderNumberHelper
    {
        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();

        public static string NewOrderNumber()
        {
            lock (_lock)
            {
                return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + _random.Next(100, 999);
            }
        }
    }
}
