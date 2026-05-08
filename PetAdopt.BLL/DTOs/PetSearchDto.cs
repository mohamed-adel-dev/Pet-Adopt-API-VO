using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class PetSearchDto
    {
        public string? AnimalType { get; set; }
        public string? Breed { get; set; }
        public int? Age { get; set; }
        public string? Location { get; set; }
    }
}
