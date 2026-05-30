using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class FavoriteResponseDto
    {
        public int PetId { get; set; }
         public string PetName { get; set; } = string.Empty;
         public string ImageUrl { get; set; } = string.Empty;
    }
}
