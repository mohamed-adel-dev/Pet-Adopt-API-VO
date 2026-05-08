using PetAdopt.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.Services.Interfaces
{
    public interface IPetService
    {
        Task<PetDto?> CreateAsync(string ownerId, string ownerName, CreatePetDto dto);
        Task<bool> DeleteAsync(int petId, string ownerId);
        Task<List<PetDto>> GetByOwnerAsync(string ownerId);
        Task<List<PetDto>> SearchAsync(PetSearchDto filter);
        Task<List<PetDto>> GetAllApprovedAsync();
        Task<PetDto?> GetByIdAsync(int petId);
        Task<PetDto> UpdateAsync(int petId, string ownerId, UpdatePetDto dto);
        Task ApprovePetAsync(int petId);
        Task RejectPetAsync(int petId);
       
    }
}
