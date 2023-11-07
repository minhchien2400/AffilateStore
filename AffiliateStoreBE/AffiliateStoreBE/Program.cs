using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Service;
using AffiliateStoreBE.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ImportProductsService>();
builder.Services.AddScoped<ImportCategoryService>();
builder.Services.AddScoped<ImportVideoReviewService>();
builder.Services.AddScoped<ICategoryService, CategorysService>();
builder.Services.AddScoped<IProductsService, ProductsService>();

builder.Services.AddDbContext<StoreDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("StoreDatabase"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
