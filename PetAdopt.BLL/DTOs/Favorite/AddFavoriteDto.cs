using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class AddFavoriteDto
    {
        [Required] public int PetId { get; set; }
    }
}
