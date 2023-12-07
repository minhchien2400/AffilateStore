using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Service;
using AffiliateStoreBE.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AffiliateStoreBE.Models;

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
builder.Services.AddScoped<IEmailService, EmailService>();


//builder.Services.AddDbContext<StoreDbContext>(option =>
//{
//    option.UseSqlServer(builder.Configuration.GetConnectionString("StoreDatabase"));
//});


builder.Services.AddDbContext<StoreDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("StoreDatabase"));
});

builder.Services.AddIdentity<Account, IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<StoreDbContext>()
    .AddDefaultTokenProviders();



// Add email config
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Add config for Required email 
builder.Services.Configure<IdentityOptions>(
    opts => opts.SignIn.RequireConfirmedEmail = true
    );

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "http://localhost:3000/login";
    options.LogoutPath = "";
});
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//                    .AddEntityFrameworkStores<AppDbContext>()
//                    .AddDefaultTokenProviders();

var app = builder.Build();
app.UseCors("AllowOrigin");

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
