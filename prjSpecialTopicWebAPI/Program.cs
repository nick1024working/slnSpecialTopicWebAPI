using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using prjSpecialTopicWebAPI.Features.Fund.Services;
using prjSpecialTopicWebAPI.Features.Shared.Options;
using prjSpecialTopicWebAPI.Features.Shared.Service;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Authentication;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Mapping;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Linq;

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
builder.Services.AddScoped<LinePayService>();

// 註冊 用於 Email 相關
// Options 綁定
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
// BLL Service
builder.Services.AddScoped<EmailService>();


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

// Shared
builder.Services.AddScoped<LinePayService>();


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
builder.Services.AddScoped<UsedBookPaymentService>();
builder.Services.AddScoped<UsedBookOrderService>();

// User
// ===== JWT 驗證設定（新增） =====
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("Missing Jwt:Key in configuration.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

//測試用
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.MapInboundClaims = false;   //  關掉自動映射，保留 'sub'、'name' 等原名

      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = signingKey,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
      };

      options.Events = new JwtBearerEvents
      {
          OnMessageReceived = ctx =>
          {
              var auth = ctx.Request.Headers.Authorization.ToString();
              if (!string.IsNullOrWhiteSpace(auth) &&
                  auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
              {
                  var bearer = auth.Substring("Bearer ".Length).Trim();
                  if (!string.IsNullOrEmpty(bearer)) { ctx.Token = bearer; return Task.CompletedTask; }
              }
              if (ctx.Request.Cookies.TryGetValue("auth_token", out var token) && !string.IsNullOrWhiteSpace(token))
              {
                  ctx.Token = token;
              }
              return Task.CompletedTask;
          },
          OnAuthenticationFailed = ctx =>
          {
              ctx.Response.Headers["x-auth-error"] = ctx.Exception.GetType().Name + ": " + ctx.Exception.Message;
              return Task.CompletedTask;
          },
          OnTokenValidated = ctx =>
          {
              // 關映射後就能直接拿到 'sub'
              var sub = ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
              ctx.Response.Headers["x-auth-ok-sub"] = sub ?? "(null)";
              return Task.CompletedTask;
          }
      };
  });


// ===== JWT 區結束 =====


// ========== 各自需要的服務於以上註冊 ==========
#endregion

builder.Services.AddControllers();

// 註冊 Swagger 服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TeamAProject API",
        Version = "v1"
    });

    // 定義 JWT Bearer 安全性方案右上角出現 Authorize
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "貼上 JWT（不用輸入 'Bearer ' 前綴）。"
    });

    // 讓所有 API 預設套用上面的安全性需求（可依需要調整）
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // 讓 Swagger 認得 DateOnly / TimeOnly
    c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
    c.MapType<DateOnly?>(() => new OpenApiSchema { Type = "string", Format = "date", Nullable = true });
    c.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string", Format = "time" });
    c.MapType<TimeOnly?>(() => new OpenApiSchema { Type = "string", Format = "time", Nullable = true });

    c.CustomSchemaIds(t => t.FullName);
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

});

// 註冊 CORS 服務與策略
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200"    // 可以用 https 跑 ng serve
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();            // 允許帶 Cookie 的跨站請求
    });
});

var app = builder.Build();

// 設定 HTTP 處理管線（Middleware）
if (app.Environment.IsDevelopment())        // 開發環境才啟動 Swagger 中介軟體
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();                  // 自動把 HTTP 轉到 HTTPS
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 回應靜態檔案時，加上 CORS header
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
    }
});
app.UseRouting();                           // 建立路由表，之後會依照路由分派

app.UseCors("AllowLocalAngular");           // 套用 CORS 策略（要放在 UseRouting 之後、UseAuthorization 之前）
app.UseSession();                           // 啟用 Session，中途可讀寫 Cookie + 狀態

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();                       // 把 Controller 的 Endpoint 加進路由表 (把路由綁定到實際控制器)
app.Run();
