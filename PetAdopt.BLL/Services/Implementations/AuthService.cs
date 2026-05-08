using Microsoft.AspNetCore.Identity;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.DAL.Entities;
using PetAdopt.DAL.Entities.Enums;

namespace PetAdopt.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {

        // Dependency injection of UserManager and SignInManager for handling user operations
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ================= REGISTER =================

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Validate input
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Check if user with the same email already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("User with the same email already exists");

            // Create new user
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                Status = UserStatus.Pending
            };

            // Create the user with the specified password
            var result = await _userManager.CreateAsync(user, dto.Password);

            
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign default role "Adopter" to the new user
            var allowedRoles = new[] { "Adopter", "Shelter" };

            if (!allowedRoles.Contains(dto.Role))
                throw new InvalidOperationException("Invalid role");

            await _userManager.AddToRoleAsync(user, dto.Role);

            // Return the authentication response with user details
            return new AuthResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = dto.Role
            };
        }

        // ================= LOGIN =================

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // Validate input
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Find user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Check approval status
            if (user.Status != UserStatus.Approved)
                throw new Exception("Account is not approved yet");

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                false);

            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Create authentication cookie
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // Return response
            return new AuthResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "Adopter"
            };
        }
    }
}