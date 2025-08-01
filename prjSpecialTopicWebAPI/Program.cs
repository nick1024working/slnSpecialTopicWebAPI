using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;

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
