using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class UpdatePetDto
    {
        public string PetName { get; set; } = string.Empty;
        public string AnimalType { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Breed { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string HealthStatus { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
