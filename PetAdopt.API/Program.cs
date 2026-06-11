using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetAdopt.API.Infrastructure.Caching;
using PetAdopt.BLL.Services.Implementations;
using PetAdopt.BLL.Services.Implementations.Caching;
using PetAdopt.BLL.Services.Implementations.JWT;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.BLL.Services.Interfaces.Caching;
using PetAdopt.BLL.Services.Interfaces.JWT;
using PetAdopt.DAL.Data;
using PetAdopt.DAL.Entities;
using PetAdopt.DAL.Entities.Enums;
using PetAdopt.DAL.Reposetories.Implementations;
using PetAdopt.DAL.Reposetories.Interfaces;
using PetAdopt.Hubs;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;

namespace PetAdopt
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connection String
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
            { 
                options.SignIn.RequireConfirmedAccount = false; 
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Redis Cache

            // Bind Redis configuration from appsettings.json to RedisConfig class
            var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisConfig>();

            // Register Redis cache services using the configuration values
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig!.Configuration;
                options.InstanceName = redisConfig.InstanceName;
            });

            // rate limiting
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("Fixed", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 100; // Maximum 100 requests
                    limiterOptions.Window = TimeSpan.FromMinutes(1); // Per 1 minute
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = 0; // No queuing, reject immediately when limit is exceeded
                });
            });


            // JWT Settings
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

            builder.Services.AddAuthentication(options =>
            {
                // Set the default authentication scheme to JWT Bearer
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                // Set the default challenge scheme to JWT Bearer (used when authentication fails)
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true; // Save the token in the AuthenticationProperties after a successful authorization
                options.RequireHttpsMetadata = true; // Disable HTTPS requirement for development/testing
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
                    };
            });

            // services
            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPetService, PetService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAdoptionRequestService, AdoptionRequestService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();

            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<ICacheService, CacheService>();



            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            builder.Services.AddAuthorization(options =>
            { });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
           // if (app.Environment.IsDevelopment())
            
                app.MapOpenApi();
                app.MapScalarApiReference();
            

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();
            // SignalR hub mapping
            app.MapHub<NotificationHub>("/notificationHub");

            // role seeding
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = new[] { "Admin", "Shelter", "Adopter" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // admin user seeding
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string email = "admin@admin.com";
                string password = "AdminTest@1234";

                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var adminUser = new ApplicationUser
                    { 
                        UserName = email,
                        Email = email,
                        FullName = "System Admin",
                        Status = UserStatus.Approved

                    };
                    await userManager.CreateAsync(adminUser, password);
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (app.Environment.IsProduction())
            {
                app.Urls.Add("http://0.0.0.0:8080");
            }

            app.Run();
        }
    }
}
