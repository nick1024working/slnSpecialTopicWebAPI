using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure
{
    public static class TestDataFactory
    {
        public static BookSaleTag CreateBookSaleTagEntity()
        {
            return new BookSaleTag { Name = "促銷標籤" + Guid.NewGuid().ToString(), IsActive = true };
        }
    }
}
