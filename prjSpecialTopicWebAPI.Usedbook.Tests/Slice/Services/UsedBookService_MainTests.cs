using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Tests.Data;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Slice.Services
{

    public class UsedBookService_MainTests : IAsyncLifetime
    {
        private UsedbookSliceTestHost _host = default!;
        private UsedBookService _svc = default!;
        private TestSeedFactory _factory = default!;
        private RandomTestSeedFactory _randFactory = default!;

        public Task InitializeAsync()
        {
            _host = new UsedbookSliceTestHost();
            _svc = _host.Services.GetRequiredService<UsedBookService>();
            _factory = new TestSeedFactory(_host.Services.GetRequiredService<TeamAProjectContext>());
            _randFactory = new RandomTestSeedFactory(_host.Services.GetRequiredService<TeamAProjectContext>());
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
            Guid sellerId = await _factory.CreateUserAsync();
            var req = TestDataFactory.GetCreateBookRequestWithoutRelationalFields();
            req.SellerDistrictId = await _factory.CreateDistrictAsync();
            req.CategoryId = await _factory.CreateBookCategoryAsync();
            req.ConditionRatingId = await _factory.CreateBookConditionRatingAsync();
            req.BindingId = await _factory.CreateBookBindingAsync();
            req.LanguageId = await _factory.CreateLanguageAsync();
            req.ContentRatingId = await _factory.CreateContentRatingAsync();

            // ---------- Act ----------
            var res = await _svc.CreateAsync(sellerId, req);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Create 結果須成功");
            var readRes = await _svc.GetPublicDetailByIdAsync(res.Value);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().BeEquivalentTo(req, opt => opt.ExcludingMissingMembers(), "request, readRes.Value 兩者需相同");
        }

        // C + U + R
        [Fact]
        public async Task Create_ThenUpdate_ThenRead_ReturnsUpdatedData()
        {
            // ---------- Arrange ----------
            Guid id = await _randFactory.CreateUsedBookAsync();
            var req = TestDataFactory.GetUpdateBookRequestWithoutRelationalFields();
            req.SellerDistrictId = await _randFactory.CreateDistrictAsync();
            req.CategoryId = await _randFactory.CreateBookCategoryAsync();
            req.ConditionRatingId = await _randFactory.CreateBookConditionRatingAsync();
            req.BindingId = await _randFactory.CreateBookBindingAsync();
            req.LanguageId = await _randFactory.CreateLanguageAsync();
            req.ContentRatingId = await _randFactory.CreateContentRatingAsync();

            // ---------- Act ----------
            var res = await _svc.UpdateAsync(id, req);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("Update 結果須成功");
            var readRes = await _svc.GetPublicDetailByIdAsync(id);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().BeEquivalentTo(req, opt => opt.ExcludingMissingMembers(), "request, readRes.Value 兩者需相同");
        }

    }
}
