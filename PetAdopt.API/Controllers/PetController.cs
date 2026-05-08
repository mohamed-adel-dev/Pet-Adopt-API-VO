using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;

namespace PetAdopt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetController(IPetService petService)
        {
            _petService = petService;
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
            try
            {
                var pet = await _petService.GetByIdAsync(id);

                if (pet == null)
                    return NotFound("Pet not found");

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

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


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

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Shelter")]
        [HttpGet("my-pets")]
        public async Task<IActionResult> GetMyPets([FromQuery] string ownerId)
        {
            var pets = await _petService.GetByOwnerAsync(ownerId);

            return Ok(pets);
        }

    }
}
