using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Errors;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Usedbook.Tests.Extensions;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Slice.Services
{

    public class BookCategoryGroupServiceTests : IAsyncLifetime
    {
        private UsedbookSliceTestHost _host = default!;
        private BookCategoryGroupService _svc = default!;

        public Task InitializeAsync()
        {
            _host = new UsedbookSliceTestHost();
            _svc = _host.Services.GetRequiredService<BookCategoryGroupService>();
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
            var req = new CreateBookCategoryGroupRequest { Name = "文學小說", IsActive = true };

            // ---------- Act ----------
            var res = await _svc.CreateAsync(req);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Create 結果須成功");
            var readRes= await _svc.GetByIdAsync(res.Value);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().BeEquivalentTo(req, opt => opt.ExcludingMissingMembers(), "request, readRes 兩者需相同");
        }

        // C + U + R
        [Fact]
        public async Task Create_ThenUpdate_ThenRead_ReturnsUpdatedData()
        {
            // ---------- Arrange ----------
            var createReq = new CreateBookCategoryGroupRequest { Name = "文學小說", IsActive = true };
            var createRes = await _svc.CreateAsync(createReq);
            createRes.IsSuccess.Should().BeTrue();

            var id = createRes.Value;
            var updateReq = new UpdatePartialBookCategoryGroupRequest { Name = "文學小小說" };

            // ---------- Act ----------
            var res = await _svc.UpdateByIdAsync(id, updateReq);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Update 結果須成功");
            var readRes = await _svc.GetByIdAsync(id);
            readRes.IsSuccess.Should().BeTrue("Read 結果須成功");
            readRes.Value.Should().BeEquivalentTo(updateReq, opt => opt.Including(req => req.Name), "updateReq, readRes 兩者需相同");
        }

        // C + R + U(order) + R
        [Fact]
        public async Task Create_ThenUpdateOrder_ThenRead_ReturnsUpdatedOrder()
        {
            await _svc.CreateAsync(new CreateBookCategoryGroupRequest { Name = "文學小說", IsActive = true });
            await _svc.CreateAsync(new CreateBookCategoryGroupRequest { Name = "社會科學", IsActive = false });
            await _svc.CreateAsync(new CreateBookCategoryGroupRequest { Name = "自然科普", IsActive = true });
            await _svc.CreateAsync(new CreateBookCategoryGroupRequest { Name = "勵志成長", IsActive = false });
            var createReadRes = await _svc.GetAllAsync();
            createReadRes.Value.Should().NotBeNull();
            var itemList = createReadRes.Value.ToList();
            itemList.Shuffle();

            var updateReqList = itemList.Select(i => new UpdateOrderByIdRequest { Id = i.Id }).ToList();

            // ---------- Act ----------
            var res = await _svc.UpdateAllOrderAsync(updateReqList);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Update 結果須成功");
            var readRes = await _svc.GetAllAsync();
            readRes.IsSuccess.Should().BeTrue("Read 結果須成功");
            readRes.Value.Should().HaveCount(itemList.Count, "應該有相同數量的項目");
            for (int i = 0; i < itemList.Count; i++)
            {
                readRes.Value[i].Id.Should().Be(itemList[i].Id, "ID 順序應該相同");
            }
        }

        // C + D + R
        [Fact]
        public async Task Create_ThenDelete_ThenRead_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            var createReq = new CreateBookCategoryGroupRequest { Name = "文學小說", IsActive = true };
            var createRes = await _svc.CreateAsync(createReq);
            createRes.IsSuccess.Should().BeTrue();

            var id = createRes.Value;

            // ---------- Act ----------
            var res = await _svc.DeleteByIdAsync(id);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Delete 結果須成功");
            var readRes = await _svc.GetByIdAsync(id);
            readRes.IsSuccess.Should().BeFalse("Read 結果須失敗");
            readRes.ErrorCode.Should().Be(ErrorCodes.General.NotFound, "錯誤碼須為 NotFound");
        }

        // C + C'
        [Fact]
        public async Task Create_DuplicateName_ReturnsConflict()
        {
            // ---------- Arrange ----------
            var createReq = new CreateBookCategoryGroupRequest { Name = "醫療保健", IsActive = true };

            // ---------- Act ----------
            await _svc.CreateAsync(createReq);
            var res = await _svc.CreateAsync(createReq);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeFalse("Create 結果須失敗");
            res.ErrorCode.Should().Be(ErrorCodes.General.Conflict, "錯誤碼須為 Conflict");
        }

        // C + D + D'
        [Fact]
        public async Task Delete_Twice_IsIdempotent()
        {
            // ---------- Arrange ----------
            var createReq = new CreateBookCategoryGroupRequest { Name = "旅遊觀光", IsActive = true };
            var createRes = await _svc.CreateAsync(createReq);
            createRes.IsSuccess.Should().BeTrue();

            var id = createRes.Value;

            // ---------- Act ----------
            await _svc.DeleteByIdAsync(id);
            var res = await _svc.DeleteByIdAsync(id);

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
            var res = await _svc.GetByIdAsync(id);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeFalse("Read 結果須失敗");
            res.ErrorCode.Should().Be(ErrorCodes.General.NotFound, "錯誤碼須為 NotFound");
        }

        // U (NotFound)
        [Fact]
        public async Task Update_NonExistingId_ReturnsNotFound()
        {
            // ---------- Arrange ----------
            int id = 9527;
            var updateReq = new UpdatePartialBookCategoryGroupRequest { Name = "生活風格", Slug="8" };

            // ---------- Act ----------
            var res = await _svc.UpdateByIdAsync(id, updateReq);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeFalse("Update 結果須失敗");
            res.ErrorCode.Should().Be(ErrorCodes.General.NotFound, "錯誤碼須為 NotFound");
        }
    }
}
