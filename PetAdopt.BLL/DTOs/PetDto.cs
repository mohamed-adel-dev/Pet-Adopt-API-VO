using PetAdopt.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class PetDto
    {
        public int Id { get; set; }

        public string OwnerId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;

        public string PetName { get; set; } = string.Empty;
        public string AnimalType { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Breed { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string HealthStatus { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
