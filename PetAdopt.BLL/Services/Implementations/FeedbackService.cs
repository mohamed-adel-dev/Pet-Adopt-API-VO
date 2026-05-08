using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.DAL.Entities;
using PetAdopt.DAL.Reposetories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        // Dependency injection of the unit of work for database operations and user manager for handling user-related operations
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public FeedbackService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<List<FeedbackDto>> GetForPetAsync(int petId)
        {
            // Validate the pet ID
            if (petId <= 0)
                throw new ArgumentException("Invalid pet ID.");

            // Retrieve feedback for the specified pet, including the adopter's information for each feedback entry
            var feedbacks = await _unitOfWork.Feedbacks
                .Query()
                 .Include(f => f.Adopter)
                .Where(f => f.PetId == petId)
                .ToListAsync();

            // Map the feedback entities to DTOs for returning to the caller
            return feedbacks.Select(f => new FeedbackDto
            {
                Id = f.Id,
                PetId = f.PetId,
                Comment = f.Comment,
                Rating = f.Rating,
                CreatedAt = f.CreatedAt,
                AdopterName = f.Adopter.FullName
            }).ToList();

        }

        public async Task<FeedbackDto?> SubmitAsync(string adopterId, CreateFeedbackDto dto)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(adopterId))
                throw new ArgumentException("Adopter ID is required.");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.PetId <= 0)
                throw new ArgumentException("Invalid pet ID.");

            if (string.IsNullOrWhiteSpace(dto.Comment))
                throw new ArgumentException("Comment cannot be empty.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            // Check if the pet exists in the database
            var pet = await _unitOfWork.Pets.GetByIdAsync(dto.PetId);
            if (pet == null)
                throw new Exception("Pet not found");

            // Check for duplicate feedback
            var exists = _unitOfWork.Feedbacks
                .Query()
                .Any(f => f.PetId == dto.PetId && f.AdopterId == adopterId);

            if (exists)
                throw new Exception("You already submitted feedback for this pet");

            var adopter = await _userManager.FindByIdAsync(adopterId);

            // Create a new feedback entity based on the provided DTO and the current date/time
            var feedback = new Feedback
            {
                PetId = dto.PetId,
                AdopterId = adopterId,
                ShelterId = pet.OwnerId,
                Comment = dto.Comment,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow
                
            };

            await _unitOfWork.Feedbacks.AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync();

            // Map the newly created feedback entity to a DTO for returning to the caller
            return new FeedbackDto
            {
                Id = feedback.Id,
                PetId = feedback.PetId,
                Comment = feedback.Comment,
                Rating = feedback.Rating,
                CreatedAt = feedback.CreatedAt,
                AdopterName = adopter?.FullName ?? "Unknown"
            };
        }
    }
}
