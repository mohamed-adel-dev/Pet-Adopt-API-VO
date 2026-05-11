using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.BLL.Services.Interfaces;

namespace PetAdopt.Controllers
{
    [Authorize(Roles = "Adopter")]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {

        // Dependency Injection of the Favorite Service
        private readonly IFavoriteService _favoriteService;
        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }


        // POST: api/Favorite/add?adopterId=123&petId=5
        [HttpPost("add")]
        public async Task<IActionResult> Add(
        [FromQuery] string adopterId,
        [FromQuery] int petId)
        {
            try
            {
                var result = await _favoriteService.AddAsync(adopterId, petId);

                return Ok(new
                {
                    Message = "Pet added to favorites successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // DELETE: api/Favorite/remove/5?adopterId=123     
        [HttpDelete("remove/{petId}")]
        public async Task<IActionResult> Remove(
        int petId,
        [FromQuery] string adopterId)
        {
            try
            {
                var result = await _favoriteService.RemoveAsync(adopterId, petId);

                if (!result)
                    return NotFound("Favorite not found");

                return Ok(new
                {
                    Message = "Favorite removed successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET: api/Favorite/my-favorites?adopterId=123
        [HttpGet("my-favorites/{adopterId}")]
        public async Task<IActionResult> GetFavorites(
        [FromQuery] string adopterId)
        {
            try
            {
                var favorites = await _favoriteService.GetFavoritesAsync(adopterId);

                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
