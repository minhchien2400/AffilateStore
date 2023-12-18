using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Service;
using AffiliateStoreBE.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AffiliateStoreBE.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

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

builder.Services.AddDbContext<StoreDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("StoreDatabase"));
});

builder.Services.AddIdentity<Account, IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<StoreDbContext>()
    .AddDefaultTokenProviders();


var configuration = builder.Configuration;

// Add email config
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Add config for Required email 
builder.Services.Configure<IdentityOptions>(
    opts =>
    {

        //
        opts.SignIn.RequireConfirmedEmail = true; // phai xac thuc account moi dang nhap duoc
        opts.SignIn.RequireConfirmedAccount = true;

        // cau hinh Lockout khi nhap sai tk mk nhieu lan
        opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        opts.Lockout.MaxFailedAccessAttempts = 5;
        opts.Lockout.AllowedForNewUsers = true;

        // cau hinh ve user 
        opts.User.RequireUniqueEmail = true; // Email la duy nhat
    });

builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
}) ;

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
    options.LoginPath = "/login";
    options.LogoutPath = "/";
});
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//                    .AddEntityFrameworkStores<AppDbContext>()
//                    .AddDefaultTokenProviders();


//login voi cac dich vu ngoai
//builder.Services.AddAuthentication().AddGoogle(options =>
//{
//    var googleConfig = configuration.GetSection("Authentication:Google");
//    options.ClientId = googleConfig["ClientId"];
//    options.ClientSecret = googleConfig["ClientSecret"];

//    options.CallbackPath = "http://localhost:3000/login-google-account";

//});

var app = builder.Build();
app.UseAuthentication();
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
