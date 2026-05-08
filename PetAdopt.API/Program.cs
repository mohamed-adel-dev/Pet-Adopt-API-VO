using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetAdopt.BLL.Services.Implementations;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.DAL.Data;
using PetAdopt.DAL.Entities;
using PetAdopt.DAL.Entities.Enums;
using PetAdopt.DAL.Reposetories.Implementations;
using PetAdopt.DAL.Reposetories.Interfaces;
using PetAdopt.Hubs;
using Scalar.AspNetCore;

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
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>
                (options => options.SignIn.RequireConfirmedAccount = false)

            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            // services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPetService, PetService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAdoptionRequestService, AdoptionRequestService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();

           

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
