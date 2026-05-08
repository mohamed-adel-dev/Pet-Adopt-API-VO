using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetAdopt.DAL.Entities;


namespace PetAdopt.DAL.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<AdoptionRequest> AdoptionRequests { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User → AdoptionRequests (one-to-many, as Owner)
            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(a => a.Owner)
                .WithMany()
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Feedback (one-to-many, as Shelter)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Shelter)
                .WithMany(f => f.ReceivedFeedbacks) 
                .HasForeignKey(f => f.ShelterId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Pets (one-to-many)
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → AdoptionRequests (one-to-many)
            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(a => a.Adopter)
                .WithMany(u => u.AdoptionRequests)
                .HasForeignKey(a => a.AdopterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pet → AdoptionRequests (one-to-many)
            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.AdoptionRequests)
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite → User (one-to-many)
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Adopter)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.AdopterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Favorite → Pet (one-to-many)
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Pet)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite — unique constraint
            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.AdopterId, f.PetId })
                .IsUnique();

            // Feedback → User (one-to-many)    
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Adopter)
                .WithMany(u => u.WrittenFeedbacks)
                .HasForeignKey(f => f.AdopterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback → Pet (one-to-many)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Pet)
                .WithMany(p => p.PetFeedbacks)
                .HasForeignKey(f => f.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback — unique constraint (1 feedback per adopter per pet)
            modelBuilder.Entity<Feedback>()
                .HasIndex(f => new { f.AdopterId, f.PetId })
                .IsUnique();

            // Enum to string conversions
            modelBuilder.Entity<ApplicationUser>()
                .Property(o => o.Status)
                .HasConversion<string>();

            // PetStatus and PostStatus are stored as strings in the database
            modelBuilder.Entity<Pet>()
             .Property(o => o.PostStatus)
             .HasConversion<string>();

            modelBuilder.Entity<Pet>()
             .Property(o => o.Status)
             .HasConversion<string>();

            // AdoptionStatus is stored as a string in the database
            modelBuilder.Entity<AdoptionRequest>()
             .Property(o => o.Status)
             .HasConversion<string>();
        }
    }
}
