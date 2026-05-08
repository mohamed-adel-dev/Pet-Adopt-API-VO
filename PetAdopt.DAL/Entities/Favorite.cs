using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.DAL.Entities
{
    public class Favorite
    {
        [Key] public int Id { get; set; }
        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [Required] public string AdopterId { get; set; } = string.Empty;
        public ApplicationUser Adopter { get; set; } = null!;

        [Required] public int PetId { get; set; } 
        public Pet Pet { get; set; } = null!;
    }
}
