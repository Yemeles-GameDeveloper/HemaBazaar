using Application.ErrorDescribers;
using Application.Extentions;
using Application.Mappings;
using Application.ValidationRules;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using HemaBazaar.MVC.Hubs;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;






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

builder.Services.AddServices();

builder.Services.AddAuthentication();

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

var app = builder.Build();

await DataSeed.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<VisitorHub>("/visitorHub");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



