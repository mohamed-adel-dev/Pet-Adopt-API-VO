using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;
using System.Text.Json;

namespace PetAdopt.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        // Dependency Injection of the Pet Service and Distributed Cache
        private readonly IPetService _petService;
        private readonly IDistributedCache _cache;
        public PetController(IPetService petService, IDistributedCache cache)
        {
            _petService = petService;
            _cache = cache;
        }


        // GET: api/Pet
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var pets = await _petService.GetAllApprovedAsync();

            return Ok(pets);
        }


        // GET: api/Pet/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cacheKey = $"Pet_{id}";

            // Try to get the pet details from cache first 
            var cachedPet = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedPet))
            {
                var petFromCache = JsonSerializer.Deserialize<PetDto>(cachedPet);

                return Ok(petFromCache);
            }

            try
            {
                var pet = await _petService.GetByIdAsync(id);

                if (pet == null)
                    return NotFound("Pet not found");

                // Store in Redis
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(pet),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow =
                            TimeSpan.FromMinutes(10)
                    });

                return Ok(pet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET: api/Pet/search?AnimalType=Dog&Breed=Labrador&Age=3&Location=NY
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] PetSearchDto dto)
        {
            var pets = await _petService.SearchAsync(dto);

            return Ok(pets);
        }


        // POST: api/Pet/create?ownerId=123&ownerName=ShelterName
        [Authorize(Roles = "Shelter")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(
                [FromQuery] string ownerId,
                [FromQuery] string ownerName,
                [FromBody] CreatePetDto dto)
        {
            try
            {
                var result = await _petService.CreateAsync(
                    ownerId,
                    ownerName,
                    dto);

                // Remove cache
                await _cache.RemoveAsync("Pets_All");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // DELETE: api/Pet/delete/5?ownerId=123
        [Authorize(Roles = "Shelter")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromQuery] string ownerId)
        {
            try
            {
                var result = await _petService.DeleteAsync(id, ownerId);

                if (!result)
                    return NotFound("Pet not found");
                
                // Remove cache
                await _cache.RemoveAsync($"Pet_{id}");

                return Ok(new
                {
                    Message = "Pet deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Pet/update/5?ownerId=123
        [Authorize(Roles = "Shelter")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(
        int id,
        [FromQuery] string ownerId,
        [FromBody] UpdatePetDto dto)
        {
            try
            {
                var result = await _petService.UpdateAsync(
                    id,
                    ownerId,
                    dto);

                // Remove cache
                await _cache.RemoveAsync($"Pet_{id}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET: api/Pet/my-pets?ownerId=123
        [Authorize(Roles = "Shelter")]
        [HttpGet("my-pets")]
        public async Task<IActionResult> GetMyPets([FromQuery] string ownerId)
        {
            var pets = await _petService.GetByOwnerAsync(ownerId);

            return Ok(pets);
        }
    }
}
