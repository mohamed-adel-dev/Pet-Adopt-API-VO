using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;

namespace PetAdopt.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {

        // Dependency Injection of the Feedback Service
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }


        // POST api/feedback/create?adopterId=123
        [Authorize(Roles = "Adopter")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(
        [FromQuery] string adopterId,
        [FromBody] CreateFeedbackDto dto)
        {
            try
            {
                var result = await _feedbackService
                    .SubmitAsync(adopterId, dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET api/feedback/pet/5
        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetForPet(int petId)
        {
            try
            {
                var feedbacks = await _feedbackService
                    .GetForPetAsync(petId);

                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
