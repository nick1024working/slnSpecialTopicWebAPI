using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
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
            Guid bookId = await _factory.CreateUsedBook();
            List<CreateUsedBookImageRequest> reqList = TestDataFactory.GetCreateUsedBookImageRequestListWithoutCover(5);
            reqList[0].IsCover = true;
            reqList.Shuffle();

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

        // C + U + R
        [Fact]
        public async Task Create_ThenUpdate_ThenRead_ReturnsUpdatedData()
        {
            // ---------- Arrange ----------
            Guid bookId = await _factory.CreateUsedBook();
            List<CreateUsedBookImageRequest> createReqList = TestDataFactory.GetCreateUsedBookImageRequestListWithoutCover(5);
            createReqList[0].IsCover = true;
            var createRes = await _svc.CreateAsync(bookId, createReqList);
            createRes.Value.Should().NotBeNull();

            List<UpdateUsedBookImageRequest> reqList = TestDataFactory.GetUpdateUsedBookImageRequestListWithoutCover(5);
            reqList[0].Id = createRes.Value[0];
            reqList[1].Id = createRes.Value[1];
            reqList[2].IsCover = true;
            reqList.Shuffle();


            // ---------- Act ----------
            var res = await _svc.UpdateByBookIdAsync(bookId, reqList);

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

        // C + D + R
        [Fact]
        public async Task Create_ThenDelete_ThenRead_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            var id = await _factory.CreateBookImage(null);

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
            var id = await _factory.CreateBookImage(null);

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

        // U (NotFound)
        [Fact]
        public async Task Update_NonExistingId_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            Guid bookId = await _factory.CreateUsedBook();
            List<UpdateUsedBookImageRequest> updateReqLList = [];

            // ---------- Act ----------
            var res = await _svc.UpdateByBookIdAsync(bookId, updateReqLList);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeFalse("Update 結果須失敗");
            res.ErrorCode.Should().Be(ErrorCodes.General.BadRequest, "錯誤碼須為 BadRequest");
        }
    }
}
