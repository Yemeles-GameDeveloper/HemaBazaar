using Application.ErrorDescribers;
using Application.Extentions;
using Application.Mappings;
using Application.ValidationRules;
using Domain.Entities;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<HemaBazaarDBContext>(options =>
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

// Add services to the container.

builder.Services.AddControllers().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<RegisterViewModelValidator>();
    fv.RegisterValidatorsFromAssemblyContaining<LoginViewModelValidator>();
    fv.AutomaticValidationEnabled = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
