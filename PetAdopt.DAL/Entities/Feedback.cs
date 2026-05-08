using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.DAL.Entities
{
    public class Feedback
    {
        [Key] public int Id { get; set; }  
        [Required, MaxLength(1000)] public string Comment { get; set; } = string.Empty;
        [Required, Range(1, 5)] public int Rating { get; set; }  // 1–5
        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Navigation
        [Required] public string ShelterId { get; set; } = string.Empty;
        public ApplicationUser Shelter { get; set; } = null!;

        [Required] public string AdopterId { get; set; } = string.Empty;
        public ApplicationUser Adopter { get; set; } = null!;

        [Required] public int PetId { get; set; }
        public Pet Pet { get; set; } = null!;

    }
}
