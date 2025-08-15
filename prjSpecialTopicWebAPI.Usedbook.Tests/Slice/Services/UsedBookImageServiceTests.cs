using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Tests.Data;
using prjSpecialTopicWebAPI.Usedbook.Tests.Extensions;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Slice.Services
{

    public class UsedBookImageServiceTests : IAsyncLifetime
    {
        private UsedbookSliceTestHost _host = default!;
        private UsedBookImageService _svc = default!;
        private TestSeedFactory _factory = default!;

        public Task InitializeAsync()
        {
            _host = new UsedbookSliceTestHost();
            _svc = _host.Services.GetRequiredService<UsedBookImageService>();
            _factory = new TestSeedFactory(_host.Services.GetRequiredService<TeamAProjectContext>());
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return _host.DisposeAsync().AsTask();
        }

        // C + R
        [Fact]
        public async Task Create_ThenRead_ReturnSameData()
        {
            // ---------- Arrange ----------
            Guid bookId = await _factory.CreateUsedBookAsync();
            List<CreateUsedBookImageRequest> reqList = TestDataFactory.GetCreateUsedBookImageRequestListWithoutCover(5);
            reqList[3].IsCover = true;

            // ---------- Act ----------
            var res = await _svc.CreateAsync(bookId, reqList);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Create 結果須成功");
            var readRes = await _svc.GetByBookIdAsync(bookId);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().HaveCount(reqList.Count, "應該有相同數量的項目");
            for (int i = 0; i < reqList.Count; i++)
            {
                readRes.Value[i].Should().BeEquivalentTo(reqList[i], opt => opt.ExcludingMissingMembers(), "reqList[i], readRes.Value[i] 兩者需相同");
            }
        }

        [Fact]
        public async Task Create_ThenGetCover_ReturnCoverData()
        {
            // ---------- Arrange ----------
            Guid bookId = await _factory.CreateUsedBookAsync();
            List<CreateUsedBookImageRequest> reqList = TestDataFactory.GetCreateUsedBookImageRequestListWithoutCover(5);
            reqList[3].IsCover = true;
            var createRes = await _svc.CreateAsync(bookId, reqList);
            createRes.IsSuccess.Should().BeTrue();

            // ---------- Act ----------
            var res = await _svc.GetCoverByBookIdAsync(bookId);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("GetCover 結果須成功");
            res.Value.Should().BeEquivalentTo(reqList[3], opt => opt.ExcludingMissingMembers(), "Cover, GetCover 兩者需相同");
        }


        // C + U(order) + R
        [Fact]
        public async Task Create_ThenUpdateOrder_ThenRead_ReturnsUpdatedOrder()
        {
            // ---------- Arrange ----------
            Guid bookId = await _factory.CreateUsedBookAsync();
            List<CreateUsedBookImageRequest> createReqList = TestDataFactory.GetCreateUsedBookImageRequestListWithoutCover(5);
            createReqList[0].IsCover = true;
            var createRes = await _svc.CreateAsync(bookId, createReqList);
            createRes.Value.Should().NotBeNull();

            var updateReq = new UpdateOrderByIdRequest();
            List<int> aux = createRes.Value.ToList();
            aux.Shuffle();
            updateReq.IdList = aux;

            // ---------- Act ----------
            var res = await _svc.UpdateOrderByBookIdAsync(bookId, updateReq);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("UpdateOrder 結果須成功");
            var readRes = await _svc.GetByBookIdAsync(bookId);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().HaveCount(updateReq.IdList.Count, "應該有相同數量的項目");
            for (int i = 0; i < updateReq.IdList.Count; i++)
            {
                readRes.Value[i].Id.Should().Be(updateReq.IdList[i], "readRes.Value[i].Id, updateReq.IdList[i] 兩者需相同");
            }
        }

        // C + D + R
        [Fact]
        public async Task Create_ThenDelete_ThenRead_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            var id = await _factory.CreateBookImageAsync(null);

            // ---------- Act ----------
            var res = await _svc.DeleteByImageIdAsync(id);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Delete 結果須成功");
            var readRes = await _svc.GetByImageIdAsync(id);
            readRes.IsSuccess.Should().BeFalse("Read 結果須失敗");
            readRes.ErrorCode.Should().Be(ErrorCodes.General.NotFound, "錯誤碼須為 NotFound");
        }

        // C + D + D'
        [Fact]
        public async Task Delete_Twice_IsIdempotent()
        {
            // ---------- Arrange ----------
            var id = await _factory.CreateBookImageAsync(null);

            // ---------- Act ----------
            await _svc.DeleteByImageIdAsync(id);
            var res = await _svc.DeleteByImageIdAsync(id);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Delete 結果須成功");
        }

        // R (NotFound)
        [Fact]
        public async Task Read_NonExistingId_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            int id = 24;

            // ---------- Act ----------
            var res = await _svc.GetByImageIdAsync(id);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeFalse("Read 結果須失敗");
            res.ErrorCode.Should().Be(ErrorCodes.General.NotFound, "錯誤碼須為 NotFound");
        }

        [Fact]
        public async Task SetCover_ThenGetCover_ReturnSameData()
        {
            // ---------- Arrange ----------
            var bookId = await _factory.CreateUsedBookAsync();
            var imageIdList = await _factory.CreateBookImageListAsync(bookId, 5);
            var setRes = await _svc.SetCoverAsync(bookId, new SetBookCoverRequest { ImageId = imageIdList.Pick() });
            setRes.IsSuccess.Should().BeTrue();

            // ---------- Act ----------
            var res = await _svc.GetCoverByBookIdAsync(bookId);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("SetCover 結果須成功");
        }
    }
}
