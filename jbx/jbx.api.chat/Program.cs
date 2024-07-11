using System.Text;
using System.Text.Json;
using jbx.api.chat.Interfaces;
using jbx.api.chat.Services;
using jbx.core.Entities.Security;
using jbx.core.Interfaces;
using jbx.core.Utils;
using jbx.infrastructure.Contexts;
using jbx.infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add connection from EntityFramework to SQL Server
builder.Services.AddDbContext<JobsityContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Jobsity"),
        b => b.MigrationsAssembly("jbx.api.chat"));
});

//Add configuration for Identity
builder.Services.AddIdentityCore<JobsityUser>(options =>
{
    options.Password = new PasswordOptions
    {
        RequiredLength = 6,
        RequireNonAlphanumeric = true,
        RequireDigit = true,
        RequireLowercase = true,
        RequireUppercase = true,
    };
    options.User = new UserOptions
    {
        RequireUniqueEmail = true,
    };
    options.Lockout = new LockoutOptions
    {
        AllowedForNewUsers = true,
        DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
        MaxFailedAccessAttempts = 5,
    };
    options.SignIn = new SignInOptions
    {
        RequireConfirmedEmail = false,
        RequireConfirmedPhoneNumber = false,
    };
}).AddEntityFrameworkStores<JobsityContext>();

var allowedOrigin = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("myAppCors", policy =>
    {
        policy.WithOrigins(allowedOrigin!)
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(optins =>
{
    optins.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["AuthSettings:Issuer"],
        ValidAudience = builder.Configuration["AuthSettings:Audience"],
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:key"] ?? "Invalid key")),
        ValidateIssuerSigningKey = true,
    };
});

builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageServices, MessageServices>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicyChat", builder =>
        builder.SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .Build());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicyChat");

app.UseAuthorization();

app.MapControllers();

app.Run();

