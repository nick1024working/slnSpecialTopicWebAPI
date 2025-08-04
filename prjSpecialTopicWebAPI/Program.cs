using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.Services;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;
using prjSpecialTopicWebAPI.Features.Usedbook.Mapping;
using prjSpecialTopicWebAPI.Models;
using prjSpecialTopicWebAPI.Usedbook.Application.Services;

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

// ���U Unit Of Work
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// ���U AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

// NOTE: ���P�B���U�b ���ձM�� UsedbookSliceTestHost ���� DI �e��
// ���U Lookup Repositories + Lookup Services
builder.Services.AddScoped<BookBindingRepository>();
builder.Services.AddScoped<BookConditionRatingRepository>();
builder.Services.AddScoped<ContentRatingRepository>();
builder.Services.AddScoped<CountyRepository>();
builder.Services.AddScoped<DistrictRepository>();
builder.Services.AddScoped<LanguageRepository>();
builder.Services.AddScoped<LookupService>();
// ���U Usedbook Repositories
builder.Services.AddScoped<BookCategoryGroupRepository>();
builder.Services.AddScoped<BookCategoryRepository>();
builder.Services.AddScoped<BookSaleTagRepository>();
// ���U Usedbook Services
builder.Services.AddScoped<BookCategoryGroupService>();
builder.Services.AddScoped<BookCategoryService>();
builder.Services.AddScoped<BookSaleTagService>();

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
