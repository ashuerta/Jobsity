using System.Configuration;
using System.Text;
using jbx.core.Entities.Security;
using jbx.core.Interfaces;
using jbx.core.Models.Rabbitmq;
using jbx.core.Utils;
using jbx.infrastructure.Middlewares;
using jbx.infrastructure.Rabbitmq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Account/Login", "");
}); ;

/*builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options => {
    options.LoginPath = "/Authentication/Login";
    options.LogoutPath = "/Authentication/Login";
});*/

/*builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["AuthSettings:Audience"],
        ValidIssuer = builder.Configuration["AuthSettings:Issuer"],
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:key"] ?? "Invalid key")),
        ValidateIssuerSigningKey = true,
    };
});*/

//builder.Services.AddIdentityCore<JobsityUser>();

//builder.Services.AddSignInManager<SignInManager<JobsityUser>>();

//builder.Services.TryAddScoped<SignInManager<JobsityUser>>();

/*builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authentication/Login";
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = evt =>
        {
            evt.Response.Redirect("/Authentication/Login");
            return Task.CompletedTask;
        }
    };
});*/

/*builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder("Cookies").RequireAuthenticatedUser().Build();
});*/

// Configure the default authorization policy
/*builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

});*/
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddTransient<HttpRequestHandler>();

builder.Services.Configure<CloudAMQP>(builder.Configuration.GetSection("amqp"));
builder.Services.AddSingleton<AmqpService>();

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<HttpRequestHandler>();

app.MapRazorPages();

app.Run();

