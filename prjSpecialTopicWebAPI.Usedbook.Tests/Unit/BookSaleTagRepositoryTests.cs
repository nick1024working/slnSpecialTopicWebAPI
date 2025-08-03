using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.Fixtures;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Unit
{
    [Collection("SqliteDbCollection")]
    public class BookSaleTagRepositoryTests
    {
        private readonly TeamAProjectContext _db;

        public BookSaleTagRepositoryTests(SqliteDbFixture dbFixture)
        {
            _db = dbFixture.DbContext;
        }

        [Fact]
        public async Task Add_Should_InsertRecord()
        {
            // Arrange
            var repo = new BookSaleTagRepository(_db);
            var entity = new BookSaleTag { Name = "促銷標籤" + Guid.NewGuid().ToString(), IsActive = true };

            // Act
            repo.Add(entity);
            await _db.SaveChangesAsync();

            // Assert
            entity.Id.Should().BeGreaterThan(0);
            var exists = await _db.BookSaleTags
                .AnyAsync(t => t.Id == entity.Id && t.Name == entity.Name && t.IsActive);
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task AddDuplicateName_Should_Throw()
        {
            // Arrange
            var repo = new BookSaleTagRepository(_db);
            var entity = new BookSaleTag { Name = "促銷標籤" + Guid.NewGuid().ToString(), IsActive = true };

            _db.BookSaleTags.Add(entity);
            await _db.SaveChangesAsync();

            // Act
            repo.Add(entity);
            Func<Task> act = () => _db.SaveChangesAsync();

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }
    }
}
