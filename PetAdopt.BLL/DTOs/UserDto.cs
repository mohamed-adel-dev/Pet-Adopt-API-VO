using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; }  = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
