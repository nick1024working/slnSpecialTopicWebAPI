using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Data
{
    public class TestDataFactory
    {
        public static CreateBookRequest GetCreateBookRequestWithoutRelationalFields()
        {
            return new CreateBookRequest
            {
                ImageList = GetCreateUsedBookImageRequestListWithCover(3),
                SalePrice = 100,
                Title = "測試書名",
                Authors = "測試作者",
                ConditionDescription = "書況良好",
                Edition = "第1版",
                Publisher = "測試出版社",
                PublicationDate = new DateOnly(2023, 10, 1),
                Isbn = "9781234567890",
                IsOnShelf = true,
            };
        }

        public static UpdateBookRequest GetUpdateBookRequestWithoutRelationalFields()
        {
            return new UpdateBookRequest
            {
                SalePrice = 200,
                Title = "測試書名2",
                Authors = "測試作者2",
                ConditionDescription = "書況不良",
                Edition = "第2版",
                Publisher = "測試出版社2",
                PublicationDate = new DateOnly(2022, 10, 1),
                Isbn = "01234567890",
                IsOnShelf = false,
            };
        }

        public static List<CreateUsedBookImageRequest> GetCreateUsedBookImageRequestListWithoutCover(int n)
        {
            if (n <= 0)
                throw new ArgumentException("n must be greater than 0", nameof(n));
            List<CreateUsedBookImageRequest> reqList = [];
            while (n-- > 0)
                reqList.Add(new CreateUsedBookImageRequest { IsCover = false, StorageProvider = StorageProvider.Local, ObjectKey = Guid.NewGuid().ToString() });
            return reqList;
        }

        public static List<CreateUsedBookImageRequest> GetCreateUsedBookImageRequestListWithCover(int n)
        {
            if (n <= 0)
                throw new ArgumentException("n must be greater than 0", nameof(n));
            List<CreateUsedBookImageRequest> reqList = [];
            while (n-- > 0)
                reqList.Add(new CreateUsedBookImageRequest { IsCover = false, StorageProvider = StorageProvider.Local, ObjectKey = Guid.NewGuid().ToString() });
            reqList[0].IsCover = true; // Set the first image as cover
            return reqList;
        }
    }
}
