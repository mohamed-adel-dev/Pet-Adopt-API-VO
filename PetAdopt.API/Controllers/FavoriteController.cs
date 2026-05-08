using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.BLL.Services.Interfaces;

namespace PetAdopt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }


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


        [Authorize(Roles = "Adopter")]
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
