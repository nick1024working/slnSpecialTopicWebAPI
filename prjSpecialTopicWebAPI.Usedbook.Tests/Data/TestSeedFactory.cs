using prjSpecialTopicWebAPI.Features.Usedbook.Enums;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Data
{
    public class TestSeedFactory
    {
        private readonly TeamAProjectContext _db;

        public TestSeedFactory(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<Guid> CreateUserAsync()
        {
            var id = Guid.NewGuid();
            var entity = new User
            {
                Uid = id,
                Phone = "0912345678",
                Password = "password123",
                Name = "測試用戶",
                Email = "test@test.com",
                Gender = true,
                Birthday = new DateOnly(1990, 1, 1),
            };
            _db.Users.Add(entity);
            await _db.SaveChangesAsync();
            return id;
        }

        public async Task<int> CreateCountyAsync()
        {
            var entity = new County { Name = "測試市" };
            _db.Counties.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateDistrictAsync()
        {
            var countyId = await CreateCountyAsync();
            var entity = new District { CountyId = countyId, Name = "測試區" };
            _db.Districts.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateLanguageAsync()
        {
            var entity = new Language { Name = "測試語言" };
            _db.Languages.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateContentRatingAsync()
        {
            var entity = new ContentRating { Name = "測試分級" };
            _db.ContentRatings.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateBookConditionRatingAsync()
        {
            var entity = new BookConditionRating { Name = "測試書況", Description= "測試書況描述" };
            _db.BookConditionRatings.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateBookBindingAsync()
        {
            var entity = new BookBinding { Name = "測試裝訂" };
            _db.BookBindings.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CreateBookCategoryAsync()
        {
            var entity = new BookCategory { Name = "測試分類", IsActive = true, DisplayOrder = 1, Slug = "1" };
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
                Title = "測試書籍",
                Authors = "測試作者",
                CategoryId = await CreateBookCategoryAsync(),
                ConditionRatingId = await CreateBookConditionRatingAsync(),
                Isbn = "9876543210000",
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
