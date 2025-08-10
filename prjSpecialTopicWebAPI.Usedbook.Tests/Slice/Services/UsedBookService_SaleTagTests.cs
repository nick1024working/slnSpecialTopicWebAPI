using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Tests.Data;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Slice.Services
{

    public class UsedBookService_SaleTagTests : IAsyncLifetime
    {
        private UsedbookSliceTestHost _host = default!;
        private UsedBookService _svc = default!;
        private BookSaleTagService _bookSaleTagService = default!;
        private TestSeedFactory _factory = default!;

        public Task InitializeAsync()
        {
            _host = new UsedbookSliceTestHost();
            _svc = _host.Services.GetRequiredService<UsedBookService>();
            _bookSaleTagService = _host.Services.GetRequiredService<BookSaleTagService>();
            _factory = new TestSeedFactory(_host.Services.GetRequiredService<TeamAProjectContext>());
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return _host.DisposeAsync().AsTask();
        }

        // U(Add) + R
        [Fact]
        public async Task AddBookSaleTag_ThenRead_ShouldContainTag()
        {
            // ---------- Arrange ----------
            var bookId = await _factory.CreateUsedBookAsync();
            var tagId = await _factory.CreateBookSaleTagAsync();

            // ---------- Act ----------
            var res = await _svc.AddBookSaleTagAsync(bookId, tagId);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("AddBookSaleTag 結果須成功");
            var readRes = await _bookSaleTagService.GetByBookIdAsync(bookId);
            readRes.IsSuccess.Should().BeTrue("Read 結果須成功");
            readRes.Value.Should().NotBeNull("Read 結果須有值");
            readRes.Value.Any(dto => dto.Id == tagId).Should().BeTrue("清單必須有 tagId");
        }

        // U(Add) + U(Remove) + R
        [Fact]
        public async Task AddBookSaleTag_ThenRemove_ThenRead_ShouldNotContainTag()
        {
            // ---------- Arrange ----------
            var bookId = await _factory.CreateUsedBookAsync();
            var tagId = await _factory.CreateBookSaleTagAsync();
            var addRes = await _svc.AddBookSaleTagAsync(bookId, tagId);
            addRes.IsSuccess.Should().BeTrue();

            // ---------- Act ----------
            var res = await _svc.RemoveBookSaleTagAsync(bookId, tagId);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("RemoveBookSaleTag 結果須成功");
            var readRes = await _bookSaleTagService.GetByBookIdAsync(bookId);
            readRes.IsSuccess.Should().BeTrue("Read 結果須成功");
            readRes.Value.Should().NotBeNull("Read 結果須有值");
            readRes.Value.Any(dto => dto.Id == tagId).Should().BeFalse("清單不該有 tagId");
        }
    }
}
