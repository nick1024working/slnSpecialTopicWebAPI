using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

            // ImageService 需要 IWebHostEnvironment
            var services = new ServiceCollection();

            services.AddSingleton<IWebHostEnvironment>(new FakeWebHostEnvironment());

            // HttpContextAccessor 與假 HttpContext ===
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor>(sp =>
            {
                var accessor = new HttpContextAccessor();
                var ctx = new DefaultHttpContext();
                ctx.Request.Scheme = "https";
                ctx.Request.Host = new HostString("localhost", 5001);
                accessor.HttpContext = ctx;
                return accessor;
            });


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
            services.AddScoped<ImageService>(sp =>
            {
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                var scheme = string.IsNullOrWhiteSpace(httpContext.Request.Scheme) ? "https" : httpContext.Request.Scheme;
                var host = httpContext.Request.Host.HasValue ? httpContext.Request.Host.Value : "localhost:5001";
                return new ImageService(env, baseUrl);
            });
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
