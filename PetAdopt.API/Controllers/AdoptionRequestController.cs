using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Implementations;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.DAL.Entities;
using PetAdopt.Hubs;

namespace PetAdopt.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdoptionRequestController : ControllerBase
    {
        private readonly IAdoptionRequestService _adoptionRequestService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IPetService _petService;

        public AdoptionRequestController(
            IAdoptionRequestService adoptionRequestService, IHubContext<NotificationHub> hubContext, IPetService petService)
        {
            _adoptionRequestService = adoptionRequestService;
            _petService = petService;
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendRequest(
    [FromQuery] string adopterId,
    [FromBody] CreateAdoptionRequestDto dto)
        {
            try
            {
                await _adoptionRequestService.SendRequestAsync(dto, adopterId);

                // Notify the pet owner about the new adoption request
                var pet = await _petService.GetByIdAsync(dto.PetId);

                await _hubContext.Clients.User(pet?.OwnerId ?? string.Empty).SendAsync(
                    "ReceiveNotification",
                    "New adoption request submitted");

                return Ok(new
                {
                    Message = "Adoption request sent successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPut("accept/{requestId}")]
        public async Task<IActionResult> AcceptRequest(
    int requestId,
    [FromQuery] string ownerId)
        {
            try
            {
                var result = await _adoptionRequestService
                    .AcceptRequestAsync(requestId, ownerId);

                if (!result)
                    return NotFound("Request not found");

                var request = await _adoptionRequestService.GetByIdAsync(requestId);
                await _hubContext.Clients.User(request?.AdopterId ?? string.Empty).SendAsync(
                    "ReceiveNotification",
                    "Your adoption request has been accepted");

                return Ok(new
                {
                    Message = "Request accepted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("reject/{requestId}")]
        public async Task<IActionResult> RejectRequest(
    int requestId,
    [FromQuery] string ownerId)
        {
            try
            {
                var result = await _adoptionRequestService
                    .RejectRequestAsync(requestId, ownerId);


                if (!result)
                    return NotFound("Request not found");

                var request = await _adoptionRequestService.GetByIdAsync(requestId);
                await _hubContext.Clients.User(request?.AdopterId ?? string.Empty).SendAsync(
                    "ReceiveNotification",
                    "Your adoption request has been rejected");

                return Ok(new
                {
                    Message = "Request rejected successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("owner-requests")]
        public async Task<IActionResult> GetOwnerRequests(
    [FromQuery] string ownerId)
        {
            try
            {
                var requests = await _adoptionRequestService
                    .GetRequestsForOwnerAsync(ownerId);

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
