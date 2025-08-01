using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Ū���s�u�r��
var connectionString = builder.Configuration.GetConnectionString("Default");

// ���U DbContext
builder.Services.AddDbContext<TeamAProjectContext>(options =>
{
    options.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(typeof(TeamAProjectContext).Assembly.FullName));
});

// ========== �U�ۻݭn���A�ȩ�H�U���U ==========
#region

// Ebook


// Forum


// Fund


// Usedbook


// User



// ========== �U�ۻݭn���A�ȩ�H�W���U ==========
#endregion

builder.Services.AddControllers();

// �[�J Swagger �A��
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// �]�w HTTP �B�z�޽u�]Middleware�^
if (app.Environment.IsDevelopment())
{
    // �ҥ� Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();

app.Run();
