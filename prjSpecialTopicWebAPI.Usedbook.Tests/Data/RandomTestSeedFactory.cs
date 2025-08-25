using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Tests.Utilities;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Data
{
    public class RandomTestSeedFactory
    {
        private readonly TeamAProjectContext _db;
        private readonly ChineseFakeGenerator _fake;

        public RandomTestSeedFactory(TeamAProjectContext db)
        {
            _db = db;
            _fake = new ChineseFakeGenerator();
        }

        public async Task<Guid> CreateUserAsync()
        {
            var id = Guid.NewGuid();
            var entity = new User
            {
                Uid = id,
                Phone = _fake.Phone(),
                Password = _fake.Password(),
                Name = _fake.Name(),
                Email = _fake.Email(),
                Gender = true,
                Birthday = new DateOnly(1990, 1, 1),
            };
            _db.Users.Add(entity);
            await _db.SaveChangesAsync();
            return id;
        }

        public async Task<int> CreateCountyAsync()
        {
            var entity = new County { Name = _fake.Name() };
            _db.Counties.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateDistrictAsync()
        {
            var countyId = await CreateCountyAsync();
            var entity = new District { CountyId = countyId, Name = _fake.Name() };
            _db.Districts.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateLanguageAsync()
        {
            var entity = new Language { Name = _fake.Name() };
            _db.Languages.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateContentRatingAsync()
        {
            var entity = new ContentRating { Name = _fake.Name() };
            _db.ContentRatings.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateBookConditionRatingAsync()
        {
            var entity = new BookConditionRating { Name = _fake.Name(), Description = _fake.Word(10, 50) };
            _db.BookConditionRatings.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateBookBindingAsync()
        {
            var entity = new BookBinding { Name = _fake.Name() };
            _db.BookBindings.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateBookSaleTagAsync()
        {
            var entity = new BookSaleTag { Name = _fake.Name(), IsActive = true, DisplayOrder = 1, Slug = _fake.Name() };
            _db.BookSaleTags.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateBookCategoryAsync()
        {
            var entity = new BookCategory { Name = _fake.Name(), IsActive = true, DisplayOrder = 1, Slug = _fake.Name() };
            _db.BookCategories.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<Guid> CreateUsedBookAsync()
        {
            var id = Guid.NewGuid();
            var entity = new UsedBook
            {
                Id = id,
                SellerId = await CreateUserAsync(),
                SellerDistrictId = await CreateDistrictAsync(),
                SalePrice = 100m,
                Title = _fake.Word(5, 10),
                Authors = _fake.Name(),
                CategoryId = await CreateBookCategoryAsync(),
                ConditionRatingId = await CreateBookConditionRatingAsync(),
                Isbn = _fake.Isbn(),
                BindingId = await CreateBookBindingAsync(),
                LanguageId = await CreateLanguageAsync(),
                ContentRatingId = await CreateContentRatingAsync(),
                IsOnShelf = true,
                IsSold = false,
                IsActive = true,
                Slug = id.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _db.UsedBooks.Add(entity);
            await _db.SaveChangesAsync();
            return id;
        }

        public async Task<int> CreateBookImageAsync(Guid? bookId)
        {
            var entity = new UsedBookImage
            {
                BookId = bookId ?? await CreateUsedBookAsync(),
                IsCover = false,
                DisplayOrder = 1,
                StorageProvider = (byte)StorageProvider.Local,
                ObjectKey = Guid.NewGuid().ToString(),
                Sha256 = Convert.FromBase64String("mZpOErm5t5R1P6zEvW+d+ZzXcW8dHZc52S9tfdlVeFY="),
                UploadedAt = DateTime.UtcNow,
            };
            _db.UsedBookImages.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<IReadOnlyList<int>> CreateBookImageListAsync(Guid? bookId, int n)
        {
            List<int> result = [];
            while (n-- > 0)
            {
                var id = await CreateBookImageAsync(bookId);
                result.Add(id);
            }
            return result;
        }
    }
}
