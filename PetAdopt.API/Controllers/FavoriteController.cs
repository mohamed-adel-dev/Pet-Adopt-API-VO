using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.BLL.Services.Interfaces.Caching;

namespace PetAdopt.Controllers
{
    [Authorize(Roles = "Adopter")]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {

        // Dependency Injection of the Favorite Service
        private readonly IFavoriteService _favoriteService;
        private readonly ICacheService _cacheService;
        public FavoriteController(IFavoriteService favoriteService, ICacheService cacheService)
        {
            _favoriteService = favoriteService;
            _cacheService = cacheService;
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

                await _cacheService.RemoveAsync($"Favorites_{adopterId}");
    
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

                await _cacheService.RemoveAsync($"Favorites_{adopterId}");
    
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
            var cacheKey = $"Favorites_{adopterId}";

            // Try to get the favorites from cache first
            var cachedFavorites = await _cacheService.GetAsync<List<FavoriteResponseDto>>(cacheKey);

            if (cachedFavorites != null)
            {
                return Ok(cachedFavorites);
            }

            try
            {
                var favorites = await _favoriteService.GetFavoritesAsync(adopterId);

                await _cacheService.SetAsync(
                    cacheKey,
                    favorites,
                    TimeSpan.FromMinutes(10));

                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
