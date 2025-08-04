using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Mapping;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// 讀取連線字串
var connectionString = builder.Configuration.GetConnectionString("Default");

// 註冊 DbContext
builder.Services.AddDbContext<TeamAProjectContext>(options =>
{
    options.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(typeof(TeamAProjectContext).Assembly.FullName));
});

// ========== 各自需要的服務於以下註冊 ==========
#region

// Ebook


// Forum


// Fund


// Usedbook

// 註冊 Unit Of Work
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// 註冊 AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

// NOTE: 須同步註冊在 測試專案 UsedbookSliceTestHost 中的 DI 容器
// 註冊 Lookup Repositories + Lookup Services
builder.Services.AddScoped<BookBindingRepository>();
builder.Services.AddScoped<BookConditionRatingRepository>();
builder.Services.AddScoped<ContentRatingRepository>();
builder.Services.AddScoped<CountyRepository>();
builder.Services.AddScoped<DistrictRepository>();
builder.Services.AddScoped<LanguageRepository>();
builder.Services.AddScoped<LookupService>();
// 註冊 Usedbook Repositories
builder.Services.AddScoped<BookCategoryGroupRepository>();
builder.Services.AddScoped<BookCategoryRepository>();
builder.Services.AddScoped<BookSaleTagRepository>();
// 註冊 Usedbook Services
builder.Services.AddScoped<BookCategoryGroupService>();
builder.Services.AddScoped<BookCategoryService>();
builder.Services.AddScoped<BookSaleTagService>();

// User



// ========== 各自需要的服務於以上註冊 ==========
#endregion

builder.Services.AddControllers();

// 加入 Swagger 服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 設定 HTTP 處理管線（Middleware）
if (app.Environment.IsDevelopment())
{
    // 啟用 Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();

app.Run();
