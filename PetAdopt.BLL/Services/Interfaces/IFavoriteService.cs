using PetAdopt.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> AddAsync(string adopterId, int petId);
        Task<bool> RemoveAsync(string adopterId, int petId);
        Task<List<PetDto>> GetFavoritesAsync(string adopterId);
    }
}
