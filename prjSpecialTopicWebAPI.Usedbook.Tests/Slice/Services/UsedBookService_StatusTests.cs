using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Tests.Data;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Slice.Services
{

    public class UsedBookService_StatusTests : IAsyncLifetime
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

        // U + R
        [Fact]
        public async Task UpdateOnShelfStatus_ThenRead_ReturnSameData()
        {
            // ---------- Arrange ----------
            Guid id = await _factory.CreateUsedBookAsync();
            var req = new UpdateStatusRequest { Value = false };

            // ---------- Act ----------
            var res = await _svc.UpdateOnShelfStatusAsync(id, req);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("UpdateOnShelfStatus 結果須成功");
            var readRes = await _svc.GetAdminDetailByIdAsync(id);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().NotBeNull("Get 結果須有值");
            readRes.Value.IsOnShelf.Should().Be(req.Value, "request, readRes.Value 兩者需相同");
        }

        // U + R
        [Fact]
        public async Task UpdateActiveStatus_ThenRead_ReturnSameData()
        {
            // ---------- Arrange ----------
            Guid id = await _factory.CreateUsedBookAsync();
            var req = new UpdateStatusRequest { Value = false };

            // ---------- Act ----------
            var res = await _svc.UpdateActiveStatusAsync(id, req);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("UpdateActiveStatus 結果須成功");
            var readRes = await _svc.GetAdminDetailByIdAsync(id);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().NotBeNull("Get 結果須有值");
            readRes.Value.IsActive.Should().Be(req.Value, "request, readRes.Value 兩者需相同");
        }


        // U + R
        [Fact]
        public async Task UpdateSoldStatus_ThenRead_ReturnSameData()
        {
            // ---------- Arrange ----------
            Guid id = await _factory.CreateUsedBookAsync();
            var req = new UpdateStatusRequest { Value = false };

            // ---------- Act ----------
            var res = await _svc.UpdateSoldStatusAsync(id, req);

            // ---------- Assert  ----------
            res.IsSuccess.Should().BeTrue("UpdateSoldStatus 結果須成功");
            var readRes = await _svc.GetAdminDetailByIdAsync(id);
            readRes.IsSuccess.Should().BeTrue("Get 結果須成功");
            readRes.Value.Should().NotBeNull("Get 結果須有值");
            readRes.Value.IsSold.Should().Be(req.Value, "request, readRes.Value 兩者需相同");
        }
    }
}
