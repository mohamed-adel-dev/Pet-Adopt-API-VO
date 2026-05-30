using PetAdopt.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.Services.Interfaces.JWT
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserDto user);
    }
}
