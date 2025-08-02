using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Unit
{
    public class BookSaleTagRepositoryTests
    {
        [Fact]
        public async Task Add_Should_InsertRecord()
        {
            // Arrange
            await using var ctx = TestDbContextFactory.CreateSqliteInMemoryContext();
            var repo = new BookSaleTagRepository(ctx);
            var entity = TestDataFactory.CreateBookSaleTagEntity();

            // Act
            repo.Add(entity);
            await ctx.SaveChangesAsync();

            // Assert
            entity.Id.Should().BeGreaterThan(0);
            var exists = await ctx.BookSaleTags
                .AnyAsync(t => t.Id == entity.Id && t.Name == entity.Name && t.IsActive);
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateRecord()
        {
            // Arrange
            await using var ctx = TestDbContextFactory.CreateSqliteInMemoryContext();

            var entityBefore = TestDataFactory.CreateBookSaleTagEntity();
            ctx.BookSaleTags.Add(entityBefore);
            await ctx.SaveChangesAsync();

            var repo = new BookSaleTagRepository(ctx);
            var entityUpdate = TestDataFactory.CreateBookSaleTagEntity();
            entityUpdate.Id = entityBefore.Id;

            // Act
            var result = await repo.UpdateAsync(entityUpdate);
            await ctx.SaveChangesAsync();

            // Assert
            result.Should().BeTrue();
            var exists = await ctx.BookSaleTags
                .AnyAsync(t => t.Id == entityUpdate.Id && t.Name == entityUpdate.Name);
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateActiveStatusAsync_Should_ToggleFlag()
        {
            // Arrange
            await using var ctx = TestDbContextFactory.CreateSqliteInMemoryContext();

            var entityBefore = TestDataFactory.CreateBookSaleTagEntity();
            ctx.BookSaleTags.Add(entityBefore);
            await ctx.SaveChangesAsync();

            var repo = new BookSaleTagRepository(ctx);

            // Act
            var result = await repo.UpdateActiveStatusAsync(entityBefore.Id, false);
            await ctx.SaveChangesAsync();

            // Assert
            result.Should().BeTrue();
            var exists = (await ctx.BookSaleTags.FindAsync(entityBefore.Id))!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task AddDuplicateName_Should_Throw()
        {
            // Arrange
            await using var ctx = TestDbContextFactory.CreateSqliteInMemoryContext();
            var repo = new BookSaleTagRepository(ctx);
            var entity = TestDataFactory.CreateBookSaleTagEntity();

            ctx.BookSaleTags.Add(entity);
            await ctx.SaveChangesAsync();

            // Act
            repo.Add(entity);
            Func<Task> act = () => ctx.SaveChangesAsync();

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }
    }
}
