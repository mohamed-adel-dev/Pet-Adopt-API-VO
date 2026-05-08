using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.DAL.Entities;
using PetAdopt.DAL.Entities.Enums;
using PetAdopt.DAL.Reposetories.Interfaces;

namespace PetAdopt.BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {

        // dependency injection of unit of work and user manager
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {          
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // ===================== USERS =====================

        public async Task<bool> ApproveUserAsync(string userId)
        {

            // validate input
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            // find user by id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // check if user is pending
            if (user.Status != UserStatus.Pending)
                throw new Exception("User already processed");
           
            user.Status = UserStatus.Approved;

            // assign default role if no roles assigned
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Any())
            {
                await _userManager.AddToRoleAsync(user, "Adopter");
            }

            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<bool> RejectUserAsync(string userId)
        {
            // validate input
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            // find user by id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // check if user is pending
            if (user.Status != UserStatus.Pending)
                throw new Exception("User already processed");

            user.Status = UserStatus.Rejected;

            await _userManager.UpdateAsync(user);
           
            return true;
        }

        public async Task<List<UserDto>> GetPendingUsersAsync()
        {

            // fetch users with pending status and map to DTO
            return await _userManager.Users
                .Where(u => u.Status == UserStatus.Pending)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    Status = u.Status.ToString()
                })
                .ToListAsync();
        }

        // ===================== PETS =====================

        public async Task<bool> ApprovePetPostAsync(int petId)
        {
            // validate input
            if (petId <= 0)
                throw new ArgumentException("Pet ID must be greater than zero", nameof(petId));

            // find pet by id
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null) return false;

            // check if pet post is pending
            if (pet.PostStatus != PostStatus.PendingApproval)
                throw new Exception("Pet already processed");

            pet.PostStatus = PostStatus.Approved;
            pet.Status = PetStatus.Available;

            await _unitOfWork.Pets.UpdateAsync(pet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectPetPostAsync(int petId)
        {
            // validate input
            if (petId <= 0)
                throw new ArgumentException("Pet ID must be greater than zero", nameof(petId));

            // find pet by id
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null) return false;

            // check if pet post is pending
            if (pet.PostStatus != PostStatus.PendingApproval)
                throw new InvalidOperationException("Pet already processed");

            pet.PostStatus = PostStatus.Rejected;

            await _unitOfWork.Pets.UpdateAsync(pet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<Pet>> GetPendingPetPostsAsync()
        {
            // fetch pets with pending post status
            return await _unitOfWork.Pets
                .Query()
                .Where(p => p.PostStatus == PostStatus.PendingApproval)
                .ToListAsync();
        } 
    }
}