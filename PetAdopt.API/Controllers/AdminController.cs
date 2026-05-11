using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.BLL.Services.Interfaces;

namespace PetAdopt.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        // Dependency Injection of the Admin Service
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        // ----------- USERS ------------

        // PUT: api/Admin/approve-user/123
        [HttpPut("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            try
            {
                var result = await _adminService.ApproveUserAsync(userId);

                if (!result)
                    return NotFound("User not found");

                return Ok("User approved successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT: api/Admin/reject-user/123
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _adminService.GetPendingUsersAsync();

            return Ok(users);
        }


        // ----------- PETS ------------


        // GET: api/Admin/pending-pets
        [HttpGet("pending-pets")]
        public async Task<IActionResult> GetPendingPets()
        {
            var pets = await _adminService.GetPendingPetPostsAsync();

            return Ok(pets);
        }


        // PUT: api/Admin/approve-pet/5
        [HttpPut("approve-pet/{petId}")]
        public async Task<IActionResult> ApprovePet(int petId)
        {
            try
            {
                var result = await _adminService.ApprovePetPostAsync(petId);

                if (!result)
                    return NotFound("Pet not found");

                return Ok("Pet approved successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT: api/Admin/reject-pet/5
        [HttpPut("reject-pet/{petId}")]
        public async Task<IActionResult> RejectPet(int petId)
        {
            try
            {
                var result = await _adminService.RejectPetPostAsync(petId);

                if (!result)
                    return NotFound("Pet not found");

                return Ok("Pet rejected successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
