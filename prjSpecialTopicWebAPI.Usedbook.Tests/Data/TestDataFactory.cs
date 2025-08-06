using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Data
{
    public class TestDataFactory
    {
        public static List<CreateUsedBookImageRequest> GetCreateUsedBookImageRequestListWithoutCover(int n)
        {
            List<CreateUsedBookImageRequest> reqList = [];
            while (n-- > 0)
                reqList.Add(new CreateUsedBookImageRequest { IsCover = false, StorageProvider = StorageProvider.Local, ObjectKey = Guid.NewGuid().ToString() });
            return reqList;
        }
    }
}
