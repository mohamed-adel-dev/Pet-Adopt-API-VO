using Microsoft.AspNetCore.Identity;
using PetAdopt.DAL.Entities.Enums;
using System.ComponentModel.DataAnnotations;


namespace PetAdopt.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)] public string FullName { get; set; } = string.Empty;
        public UserStatus Status { get; set; } = UserStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Navigation

        // For shelters, pets they have listed
        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();
        // For adopters, pets they have favorited
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        // For adopters, feedbacks given to shelters
        public ICollection<Feedback> WrittenFeedbacks { get; set; } = new List<Feedback>();
        // For shelters, feedbacks received from adopters
        public ICollection<Feedback> ReceivedFeedbacks { get; set; } = new List<Feedback>();

    }
}
