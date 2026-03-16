using Application.ErrorDescribers;
using Application.Extentions;
using Application.Mappings;
using Application.ValidationRules;
using Domain.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using HemaBazaar.MVC.Hubs;
using HemaBazaar.MVC.Middlewares;
using HemaBazaar.MVC.Models;
using HemaBazaar.MVC.Services;
using Infrastructure.Data;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;






var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HemaBazaarDBContext>( options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HemaBazaarDB"));
});

builder.Services.AddDbContext<HemaBazaarLogDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HemaBazaarLogDB"));
});


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});


builder.Services.AddMemoryCache();
builder.Services.AddOutputCache(opt=>
{
    opt.AddBasePolicy(pol =>
    {
        pol.Expire(TimeSpan.FromSeconds(50));
    });
});

builder.Services.AddServices();

//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
// Add services to the container.

builder.Services.AddControllersWithViews().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<RegisterViewModelValidator>();
    fv.RegisterValidatorsFromAssemblyContaining<LoginViewModelValidator>();
    fv.AutomaticValidationEnabled = true;
});


     


builder.Services
    .AddIdentity<AppUser, AppRole>(opt =>
    {
        opt.Password.RequireDigit = true;
        opt.Password.RequireNonAlphanumeric = true;
        opt.Password.RequireDigit = true;
        opt.Password.RequiredLength = 6;
        opt.Password.RequireUppercase = true;

        //Register'da bu kontrolleri eklemeyi unutma.


        opt.SignIn.RequireConfirmedEmail = true;


        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        opt.Lockout.AllowedForNewUsers = true;

    }
    )
    .AddEntityFrameworkStores<HemaBazaarDBContext>()
    .AddErrorDescriber<EnglishIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

    builder.Services.AddControllersWithViews();

    builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

    builder.Services.AddValidatorsFromAssemblyContaining<RegisterViewModelValidator>();


builder.Services.AddSignalR();

var redisConfig = builder.Configuration.GetSection("Redis");

builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = $"{redisConfig["Host"]}:{redisConfig["Port"]},abortConnect=false,connectTimeout=5000,syncTimeout=5000";
    opt.InstanceName = redisConfig["InstanceName"];
});

builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
{
    var config = $"{redisConfig["Host"]}:{redisConfig["Port"]},abortConnect=false,connectTimeout=5000,syncTimeout=5000";
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = false
});

builder.Services.AddScoped(typeof(RedisCacheService<>));

    builder.Services.Configure<IyzicoOptions>(builder.Configuration.GetSection("IyzicoOptions"));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(typeof(TokenServices));
builder.Services.AddScoped(typeof(ApiClient));
builder.Services.AddScoped<MvcJwtTokenService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Error/Unauthorized";
});


var app = builder.Build();

await DataSeed.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<CustomErrorMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapHub<VisitorHub>("/visitorHub");

//app.MapAreaControllerRoute(
//    name: "Admin",
//    areaName: "Admin",
//    pattern: "{area=Admin}/{controller=Dashboard}/{action=Index}/{id?}"

//    );

app.MapControllerRoute(
     name: "areas",
     pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
   );


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();





