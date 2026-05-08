using PetAdopt.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.DAL.Entities
{
    public class AdoptionRequest
    {
        [Key]public int Id { get; set; } 
        [Required]public AdoptionStatus Status { get; set; } = AdoptionStatus.Pending;
        [Required, MaxLength(500)]public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [Required] public int PetId { get; set; }
        public Pet Pet { get; set; } = null!;

        [Required] public string AdopterId { get; set; } = string.Empty;
        public ApplicationUser Adopter { get; set; } = null!;

        [Required]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;

    }
}
