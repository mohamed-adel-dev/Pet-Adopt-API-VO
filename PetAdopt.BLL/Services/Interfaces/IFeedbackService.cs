using PetAdopt.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackDto?> SubmitAsync(string adopterId, CreateFeedbackDto dto);
        Task<List<FeedbackDto>> GetForPetAsync(int petId);
    }
}
