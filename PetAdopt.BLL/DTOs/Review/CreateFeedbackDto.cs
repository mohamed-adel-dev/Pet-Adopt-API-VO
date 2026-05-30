using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class CreateFeedbackDto
    {
        [Required] public int PetId { get; set; }
        [Required, MaxLength(1000)] public string Comment { get; set; } = string.Empty;
        [Required, Range(1, 5)] public int Rating { get; set; }  // 1–5
   
    }
}
