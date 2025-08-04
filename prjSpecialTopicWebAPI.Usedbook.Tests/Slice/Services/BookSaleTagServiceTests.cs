using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Slice.Services
{

    public class BookSaleTagServiceTests : IAsyncLifetime
    {
        private UsedbookSliceTestHost _host = default!;
        private BookSaleTagService _svc = default!;

        public Task InitializeAsync()
        {
            _host = new UsedbookSliceTestHost();
            _svc = _host.Services.GetRequiredService<BookSaleTagService>();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return _host.DisposeAsync().AsTask();
        }

        // C + R
        [Fact]
        public async Task Create_ThenReadt_ReturnSameData()
        {
            // ---------- Arrange ----------
            var svc = _svc;
            var req = new CreateSaleTagRequest { Name = "限時特價CR", IsActive = true };

            // ---------- Act ----------
            var res = await svc.CreateAsync(req);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Create 結果須成功");
            var readRes= await svc.GetByIdAsync(res.Value);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().BeEquivalentTo(req, opt => opt.ExcludingMissingMembers(), "request, readRes 兩者需相同");
        }

        // C + U + R
        [Fact]
        public async Task Create_ThenUpdate_ThenRead_ReturnsUpdatedData()
        {
            // ---------- Arrange ----------
            var svc = _svc;
            var createReq = new CreateSaleTagRequest { Name = "夜間特價CUR", IsActive = true };
            var createRes = await svc.CreateAsync(createReq);
            createRes.IsSuccess.Should().BeTrue();

            var id = createRes.Value;
            var updateReq = new UpdatePartialBookSaleTagRequest { Name = "限時夜殺CUR" };

            // ---------- Act ----------
            var res = await svc.UpdateByIdAsync(id, updateReq);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Update 結果須成功");
            var readRes = await svc.GetByIdAsync(id);
            readRes.IsSuccess.Should().BeTrue("Read 結果須成功");
            readRes.Value.Should().BeEquivalentTo(updateReq, opt => opt.Including(req => req.Name), "updateReq, readRes 兩者需相同");
        }

        // C + D + R
        [Fact]
        public async Task Create_ThenDelete_ThenRead_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            var svc = _svc;
            var createReq = new CreateSaleTagRequest { Name = "編輯推薦CDR", IsActive = true };
            var createRes = await svc.CreateAsync(createReq);
            createRes.IsSuccess.Should().BeTrue();

            var id = createRes.Value;

            // ---------- Act ----------
            var res = await svc.DeleteByIdAsync(id);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Delete 結果須成功");
            var readRes = await svc.GetByIdAsync(id);
            readRes.IsSuccess.Should().BeFalse("Read 結果須失敗");
            readRes.ErrorCode.Should().Be(ErrorCodes.General.NotFound, "錯誤碼須為 NotFound");
        }

        // C + C'
        [Fact]
        public async Task Create_DuplicateName_ReturnsConflict()
        {
            // ---------- Arrange ----------
            var svc = _svc;
            var createReq = new CreateSaleTagRequest { Name = "本月新書CC", IsActive = true };

            // ---------- Act ----------
            await svc.CreateAsync(createReq);
            var res = await svc.CreateAsync(createReq);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeFalse("Create 結果須失敗");
            res.ErrorCode.Should().Be(ErrorCodes.General.Conflict, "錯誤碼須為 Conflict");
        }

        // C + D + D'
        [Fact]
        public async Task Delete_Twice_IsIdempotent()
        {
            // ---------- Arrange ----------
            var svc = _svc;
            var createReq = new CreateSaleTagRequest { Name = "即將下架CDD", IsActive = true };
            var createRes = await svc.CreateAsync(createReq);
            createRes.IsSuccess.Should().BeTrue();

            var id = createRes.Value;

            // ---------- Act ----------
            await svc.DeleteByIdAsync(id);
            var res = await svc.DeleteByIdAsync(id);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Delete 結果須成功");
        }

        // R (NotFound)
        [Fact]
        public async Task Read_NonExistingId_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            var svc = _svc;
            int id = 9527;

            // ---------- Act ----------
            var res = await svc.GetByIdAsync(id);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeFalse("Read 結果須失敗");
            res.ErrorCode.Should().Be(ErrorCodes.General.NotFound, "錯誤碼須為 NotFound");
        }

        // U (NotFound)
        [Fact]
        public async Task Update_NonExistingId_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            var svc = _svc;
            int id = 9527;
            var updateReq = new UpdatePartialBookSaleTagRequest { Name = "錯過可惜U" };

            // ---------- Act ----------
            var res = await svc.UpdateByIdAsync(id, updateReq);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeFalse("Update 結果須失敗");
            res.ErrorCode.Should().Be(ErrorCodes.General.NotFound, "錯誤碼須為 NotFound");
        }
    }
}
