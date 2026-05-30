using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class CreateAdoptionRequestDto
    {
        [Required] public int PetId { get; set; }    
        [MaxLength(500)] public string Message { get; set; } = string.Empty;
    }
}
