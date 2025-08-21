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

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS：允許 Angular Dev Server
const string AllowAngular = "AllowAngular";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(AllowAngular, p => p
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
    // .AllowCredentials() // 需要帶 cookie/token 時再開
    );
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

// ★ 關鍵：一定要加在 MapControllers 之前
app.UseCors(AllowAngular);

app.UseAuthorization();
app.MapControllers();

app.Run();
