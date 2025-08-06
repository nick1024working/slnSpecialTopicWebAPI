using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Tests.Data;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Slice.Services
{

    public class UsedBookServiceTests : IAsyncLifetime
    {
        private UsedbookSliceTestHost _host = default!;
        private UsedBookService _svc = default!;
        private TestSeedFactory _factory = default!;

        public Task InitializeAsync()
        {
            _host = new UsedbookSliceTestHost();
            _svc = _host.Services.GetRequiredService<UsedBookService>();
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
            Guid sellerId = await _factory.CreateUserAsync();
            var req = TestDataFactory.GetCreateBookRequest();
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
            var readRes = await _svc.GetPubicDetailAsync(res.Value);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().BeEquivalentTo(req, opt => opt.ExcludingMissingMembers(), "request, readRes.Value 兩者需相同");
        }

        // C + U + R
        //[Fact]
        //public async Task Create_ThenUpdateOrder_ThenRead_ReturnsUpdatedOrder()
        //{
        //}

    }
}
