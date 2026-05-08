using PetAdopt.BLL.DTOs;
using PetAdopt.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.Services.Interfaces
{
    public interface IAdoptionRequestService
    {
        Task SendRequestAsync(CreateAdoptionRequestDto request, string adopterId);
        Task<bool> AcceptRequestAsync(int requestId, string ownerId);
        Task<bool> RejectRequestAsync(int requestId, string ownerId);
        Task<List<AdoptionRequestDto>> GetRequestsForOwnerAsync(string ownerId);
        Task<AdoptionRequestDto?> GetByIdAsync(int requestId);
    }
}
