using Jobsity.Core.Entity;
using Jobsity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;

namespace Jobsity.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddDbContext<JobsityContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Jobsity")));
            services.AddIdentity<JobsityUser, IdentityRole>()
                .AddEntityFrameworkStores<JobsityContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "http://ashuerta.com",
                    ValidIssuer = "http://ashuerta.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abraham.huerta@jobsity.com"))
                };
            });
            RegisterServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var bdInstall = Configuration.GetValue<bool>("Development:DbInstall");

            if (bdInstall)
            {
                app.Run(async (context) =>
                {
                    var appCtx = context.RequestServices.GetRequiredService<JobsityContext>();

                    await appCtx.Database.EnsureDeletedAsync().ConfigureAwait(true);
                    logger.LogInformation("Deleting... Database");
                    await appCtx.Database.EnsureCreatedAsync().ConfigureAwait(true);
                    logger.LogInformation("Creating... Database");
                    if (appCtx.Database.CanConnect())
                    {
                        if (appCtx.Database.IsSqlServer())
                        {
                            if (!appCtx.Users.Any())
                            {
                                var userManager = context.RequestServices.GetRequiredService<UserManager<JobsityUser>>();
                                JobsityUser user = new JobsityUser()
                                {
                                    Email = "ashuerta@hotmail.com",
                                    SecurityStamp = Guid.NewGuid().ToString(),
                                    UserName = "ashuerta"
                                };
                                await userManager.CreateAsync(user, "Jobsity@123");
                                logger.LogInformation("User Initializing...");
                                await context.Response.WriteAsync("Done!").ConfigureAwait(true);
                                await context.Response.WriteAsync(Environment.NewLine).ConfigureAwait(true);
                                await context.Response.WriteAsync("Navigate to /healthCheck to see the health status.").ConfigureAwait(true);
                                await context.Response.WriteAsync(Environment.NewLine).ConfigureAwait(true);
                            }
                        }
                    }
                });
            }

            app.UseRouting();

            app.UseCors(b =>
                b.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("http://localhost:7001")
                .AllowCredentials()
            );


            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<SecurityRepository, SecurityRepository>();
        }
    }
}
