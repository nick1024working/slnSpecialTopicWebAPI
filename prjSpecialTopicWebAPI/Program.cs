using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Features.Fund.Services;
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
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IfundImageService, fundImageService>();
builder.Services.AddScoped<IPlanService, PlanService>();


// Usedbook

// ���U Unit Of Work
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// ���U AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

// NOTE: ���P�B���U�b ���ձM�� UsedbookSliceTestHost ���� DI �e��
// ���U ImageService
builder.Services.AddScoped<ImageService>();
// ���U Lookups Repo & Svc
builder.Services.AddScoped<BookBindingRepository>();
builder.Services.AddScoped<BookConditionRatingRepository>();
builder.Services.AddScoped<ContentRatingRepository>();
builder.Services.AddScoped<CountyRepository>();
builder.Services.AddScoped<DistrictRepository>();
builder.Services.AddScoped<LanguageRepository>();
builder.Services.AddScoped<LookupService>();
// ���U ���� + ���� Repo & Svc
builder.Services.AddScoped<BookCategoryRepository>();
builder.Services.AddScoped<BookSaleTagRepository>();
builder.Services.AddScoped<BookCategoryService>();
builder.Services.AddScoped<BookSaleTagService>();
// ���U �ѥ��֤� Repo & Svc
builder.Services.AddScoped<UsedBookImageRepository>();
builder.Services.AddScoped<UsedBookRepository>();
//builder.Services.AddScoped<UsedBookOrderRepository>();
builder.Services.AddScoped<UsedBookImageService>();
builder.Services.AddScoped<UsedBookService>();
//builder.Services.AddScoped<UsedBookOrderService>();

// User



// ========== �U�ۻݭn���A�ȩ�H�W���U ==========
#endregion

builder.Services.AddControllers();

// ���U Swagger �A��
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ���U CORS �A�ȻP����
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // ���\ Angular �e��
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// �ҥ� CORS
app.UseCors();

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
