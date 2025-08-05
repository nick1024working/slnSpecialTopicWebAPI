using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Slice.Services
{

    public class ImageServiceTests : IAsyncLifetime
    {
        private UsedbookSliceTestHost _host = default!;

        public Task InitializeAsync()
        {
            _host = new UsedbookSliceTestHost();
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

            // ---------- Act ----------

            // ---------- Assert  ----------
        }

        // C + U + R
        [Fact]
        public async Task Create_ThenUpdate_ThenRead_ReturnsUpdatedData()
        {
            // ---------- Arrange ----------

            // ---------- Act ----------

            // ---------- Assert  ----------
        }

        // C + R + U(order) + R
        [Fact]
        public async Task Create_ThenUpdateOrder_ThenRead_ReturnsUpdatedOrder()
        {
            // ---------- Arrange ----------

            // ---------- Act ----------

            // ---------- Assert  ----------
        }

        // C + D + R
        [Fact]
        public async Task Create_ThenDelete_ThenRead_ReturnsNotFound()
        {
            // ---------- Arrange ----------

            // ---------- Act ----------

            // ---------- Assert  ----------
        }

        // C + C'
        [Fact]
        public async Task Create_DuplicateName_ReturnsConflict()
        {
            // ---------- Arrange ----------

            // ---------- Act ----------

            // ---------- Assert  ----------
        }

        // C + D + D'
        [Fact]
        public async Task Delete_Twice_IsIdempotent()
        {
            // ---------- Arrange ----------

            // ---------- Act ----------

            // ---------- Assert  ----------
        }

        // R (NotFound)
        [Fact]
        public async Task Read_NonExistingId_ReturnsNotFound()
        {
            // ---------- Arrange ----------

            // ---------- Act ----------

            // ---------- Assert  ----------
        }

        // U (NotFound)
        [Fact]
        public async Task Update_NonExistingId_ReturnsNotFound()
        {
            // ---------- Arrange ----------

            // ---------- Act ----------

            // ---------- Assert  ----------
        }
    }
}
