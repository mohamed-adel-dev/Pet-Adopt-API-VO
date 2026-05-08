using PetAdopt.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.DAL.Entities
{
    public class Pet
    {
        [Key]public int Id { get; set; }

        [Required, MaxLength(100)] public string OwnerName { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string PetName { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string AnimalType { get; set; } = string.Empty;  
        [Required] public int Age { get; set; }
        [Required, MaxLength(50)] public string Breed { get; set; } = string.Empty;
        [Required, MaxLength(10)] public string Gender { get; set; } = string.Empty;       
        [Required, MaxLength(100)] public string HealthStatus { get; set; } = string.Empty;
        [MaxLength(500)] public string Description { get; set; } = string.Empty;
        [MaxLength(100)] public string Location { get; set; } = string.Empty;
        [MaxLength(200)] public string ImageUrl { get; set; } = string.Empty;
 
        [Required] public PetStatus Status { get; set; } = PetStatus.Available;
        [Required] public PostStatus PostStatus { get; set; } = PostStatus.PendingApproval;

        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [Required] public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;

        // Navigation

        // Adoption requests made by adopters for this pet
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();
        // Adopters who have favorited this pet
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        // Feedbacks given by adopters for this pet (and its shelter)
        public ICollection<Feedback> PetFeedbacks { get; set; } = new List<Feedback>();
    }
}
