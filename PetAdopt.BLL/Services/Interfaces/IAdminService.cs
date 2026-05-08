using PetAdopt.BLL.DTOs;
using PetAdopt.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> ApproveUserAsync(string userId);
        Task<bool> RejectUserAsync(string userId);

        Task<bool> ApprovePetPostAsync(int petId);
        Task<bool> RejectPetPostAsync(int petId);

        Task<List<UserDto>> GetPendingUsersAsync();
        Task<List<Pet>> GetPendingPetPostsAsync();
    }
}
