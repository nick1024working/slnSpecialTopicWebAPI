using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using prjSpecialTopicWebAPI.Features.Shared.Controllers;
using prjSpecialTopicWebAPI.Features.Shared.Options;
using prjSpecialTopicWebAPI.Features.Shared.Service;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Authentication;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Mapping;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 讀取連線字串
var connectionString = builder.Configuration.GetConnectionString("Default");

// 註冊 DbContext
builder.Services.AddDbContext<TeamAProjectContext>(options =>
{
    options.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(typeof(TeamAProjectContext).Assembly.FullName));
});


// 註冊 用於 LinePay API 相關
// Options 綁定
builder.Services.Configure<LinePayOptions>(builder.Configuration.GetSection("Payments:LinePay"));
// HttpClient（命名客戶端）
builder.Services.AddHttpClient("LinePay", (sp, c) =>
{
    var opt = sp.GetRequiredService<IOptions<LinePayOptions>>().Value;
    c.BaseAddress = new Uri(opt.BaseAddress);
    c.Timeout = TimeSpan.FromSeconds(20);
});

// BLL Service
builder.Services.AddScoped<PaymentService>();

// 註冊 DataProtection
builder.Services.AddDataProtection();

// 註冊記憶體快取 目前用於 session
builder.Services.AddDistributedMemoryCache();
// 註冊 Session 目前用於未登錄購物車
builder.Services.AddSession(opts =>
{
    opts.Cookie.Name = ".Session";
    opts.IdleTimeout = TimeSpan.FromMinutes(30);
    opts.Cookie.HttpOnly = true;
    opts.Cookie.SameSite = SameSiteMode.None;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// 註冊單例 Random
builder.Services.AddSingleton<Random>();

// ========== 各自需要的服務於以下註冊 ==========
#region

// Ebook


// Forum


// Fund
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IfundImageService, fundImageService>();
builder.Services.AddScoped<IPlanService, PlanService>();


// Usedbook

// 設定 EPPlus 授權模式
ExcelPackage.License.SetNonCommercialOrganization("MSIT-TeamA");


builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });
builder.Services.AddScoped<ExcelService>();
builder.Services.AddSingleton<AuthHelper>();

// 註冊 ImageService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ImageService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
    return new ImageService(env, baseUrl);
});
// 註冊 Lookups Repo & Svc
builder.Services.AddScoped<BookBindingRepository>();
builder.Services.AddScoped<BookConditionRatingRepository>();
builder.Services.AddScoped<ContentRatingRepository>();
builder.Services.AddScoped<CountyRepository>();
builder.Services.AddScoped<DistrictRepository>();
builder.Services.AddScoped<LanguageRepository>();
builder.Services.AddScoped<LookupService>();
//
builder.Services.AddScoped<ExternalDomainRepository>();
builder.Services.AddScoped<ExternalDomainService>();
// 註冊 分類 + 標籤 Repo & Svc
builder.Services.AddScoped<BookCategoryRepository>();
builder.Services.AddScoped<BookSaleTagRepository>();
builder.Services.AddScoped<BookCategoryService>();
builder.Services.AddScoped<BookSaleTagService>();
// 註冊 書本核心 Repo & Svc
builder.Services.AddScoped<UsedBookImageRepository>();
builder.Services.AddScoped<UsedBookRepository>();
builder.Services.AddScoped<UsedBookOrderRepository>();
builder.Services.AddScoped<UsedBookImageService>();
builder.Services.AddScoped<UsedBookService>();
builder.Services.AddScoped<UsedBookOrderService>();

// 註冊 LinePayController
builder.Services.AddScoped<PaymentController>();

// User
// ===== JWT 驗證設定（新增） =====
var jwtKey = builder.Configuration["Jwt:Key"] ?? "PLEASE_REPLACE_WITH_A_LONG_RANDOM_SECRET"; //  開發用可先寫固定字串
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //  啟用 JWT
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,   // Demo 先關
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey
        };
    });
// ===== JWT 區結束 =====


// ========== 各自需要的服務於以上註冊 ==========
#endregion

builder.Services.AddControllers();

// 註冊 Swagger 服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 註冊 CORS 服務與策略
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // 允許 Angular 前端
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();

    });
});

var app = builder.Build();

// 啟用 CORS
app.UseCors();

// 設定 HTTP 處理管線（Middleware）
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
