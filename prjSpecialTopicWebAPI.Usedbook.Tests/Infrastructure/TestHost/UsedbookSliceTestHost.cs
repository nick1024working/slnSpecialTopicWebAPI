using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Mapping;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.TestHost
{
    /// <summary>
    /// 提供每次測試的獨立 DI 與資料庫容器
    /// </summary>
    public class UsedbookSliceTestHost : IAsyncDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly ServiceProvider _provider;
        private readonly IServiceScope _scope;

        public IServiceProvider Services => _scope.ServiceProvider;

        public UsedbookSliceTestHost()
        {
            // 為每個測試建立獨立記憶體資料庫
            var dbName = Guid.NewGuid().ToString();
            var connStr = $"Filename={dbName};Mode=Memory;Cache=Shared";

            _conn = new SqliteConnection(connStr);
            _conn.Open();

            var services = new ServiceCollection();

            services.AddScoped(_ =>
            {
                var options = new DbContextOptionsBuilder<TeamAProjectContext>()
                    .UseSqlite(_conn)
                    .EnableSensitiveDataLogging()
                    .Options;

                var ctx = new TeamAProjectContext(options);
                ctx.Database.EnsureCreated();
                return ctx;
            });

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            services.AddScoped<BookCategoryRepository>();
            services.AddScoped<BookSaleTagRepository>();
            services.AddScoped<UsedBookImageRepository>();
            services.AddScoped<UsedBookRepository>();

            services.AddScoped<BookCategoryService>();
            services.AddScoped<BookSaleTagService>();
            services.AddScoped<ImageService>();
            services.AddScoped<UsedBookImageService>();
            services.AddScoped<UsedBookService>();

            services.AddLogging();

            _provider = services.BuildServiceProvider();
            _scope = _provider.CreateScope();
        }

        public T GetService<T>() where T : notnull => Services.GetRequiredService<T>();

        public async ValueTask DisposeAsync()
        {
            _scope.Dispose();
            await _provider.DisposeAsync();
            await _conn.DisposeAsync();
        }
    }
}
