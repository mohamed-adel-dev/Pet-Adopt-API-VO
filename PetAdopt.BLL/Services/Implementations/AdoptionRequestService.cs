using Microsoft.EntityFrameworkCore;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.DAL.Entities.Enums;
using PetAdopt.DAL.Reposetories.Interfaces;


namespace PetAdopt.BLL.Services.Implementations
{
    public class AdoptionRequestService : IAdoptionRequestService
    {

        // dependency injection of unit of work
        private readonly IUnitOfWork _unitOfWork;

        public AdoptionRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<bool> AcceptRequestAsync(int requestId, string ownerId)
        {
            // validate request ID
            if (requestId <= 0)
                throw new ArgumentException("Invalid request ID.");

            //  validate owner ID
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException("Owner ID is required.");

            // get request with pet details
            var request = await _unitOfWork.AdoptionRequests
                .Query()
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r =>
                    r.Id == requestId &&
                    r.Pet.OwnerId == ownerId);

            if (request == null)
                return false;

            // only pending requests can be accepted
            if (request.Status != AdoptionStatus.Pending)
                throw new Exception("Request already processed");

            // check if pet is already adopted
            if (request.Pet.Status == PetStatus.Adopted)
                throw new Exception("Pet already adopted");

            // accept selected request
            request.Status = AdoptionStatus.Accepted;

            // pet adopted
            request.Pet.Status = PetStatus.Adopted;

            // reject other requests
            var otherRequests = await _unitOfWork.AdoptionRequests
                .Query()
                .Where(r =>
                    r.PetId == request.PetId &&
                    r.Id != request.Id)
                .ToListAsync();

            // reject all other pending requests for the same pet
            foreach (var item in otherRequests)
            {
                if (item.Status == AdoptionStatus.Pending)
                {
                    item.Status = AdoptionStatus.Rejected;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<AdoptionRequestDto?> GetByIdAsync(int requestId)
        {
            var request = await _unitOfWork.AdoptionRequests
                .Query()
                .Include(r => r.Adopter)
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                return null;

            return new AdoptionRequestDto
            {
                Id = request.Id,
                AdopterId = request.AdopterId,
                PetId = request.PetId,
                PetName = request.Pet.PetName,
                AdopterName = request.Adopter.FullName,
                Status = request.Status.ToString(),
                Message = request.Message,
                CreatedAt = request.CreatedAt
            };
        }

        public async Task<List<AdoptionRequestDto>> GetRequestsForOwnerAsync(string ownerId)
        {
            // validate owner ID
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException("Owner ID is required.");

            // get all requests for pets owned by the owner with pet and adopter details & map to DTO
            var requests = await _unitOfWork.AdoptionRequests
                  .Query()
                 .Include(r => r.Pet)
                 .Include(r => r.Adopter)
                .Where(r => r.Pet.OwnerId == ownerId)
                .Select(r => new AdoptionRequestDto
                {
                    Id = r.Id,
                    PetId = r.PetId,
                    PetName = r.Pet.PetName,
                    AdopterName = r.Adopter.FullName,
                    Status = r.Status.ToString(),
                    Message = r.Message,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return requests;
        }

        public async Task<bool> RejectRequestAsync(int requestId, string ownerId)
        {
            // validate request ID
            if (requestId <= 0)
                throw new ArgumentException("Invalid request ID.");

            // validate owner ID
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException("Owner ID is required.");

            // get request with pet details
            var request = await _unitOfWork.AdoptionRequests
                .Query()
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r =>
                    r.Id == requestId &&
                    r.Pet.OwnerId == ownerId);

            if (request == null)
                return false;

            // only pending requests can be rejected
            if (request.Status != AdoptionStatus.Pending)
                throw new Exception("Request already processed");


            request.Status = AdoptionStatus.Rejected;
            await _unitOfWork.SaveChangesAsync();

            return true;

        }

        public async Task SendRequestAsync(CreateAdoptionRequestDto request, string adopterId)
        {
            // validate adopter ID
            if (string.IsNullOrEmpty(adopterId))
                throw new ArgumentException("Adopter ID is required.");

            // validate request
            if (request == null)
                throw new ArgumentException("Request data is required.");

            // check pet exists
            var pet = await _unitOfWork.Pets.GetByIdAsync(request.PetId);

            if (pet == null)
                throw new Exception("Pet not found");

            // check pet approved
            if (pet.PostStatus != PostStatus.Approved)
                throw new Exception("Pet not approved");

            // check already adopted
            if (pet.Status == PetStatus.Adopted)
                throw new Exception("Pet already adopted");

            // prevent owner from adopting own pet
            if (pet.OwnerId == adopterId)
                throw new Exception("You cannot adopt your own pet");

            // prevent duplicate requests
            var exists = await _unitOfWork.AdoptionRequests
                .Query()
                .AnyAsync(r =>
                    r.PetId == request.PetId &&
                    r.AdopterId == adopterId);

            if (exists)
                throw new Exception("Request already exists");

            // create new request
            var newRequest = new DAL.Entities.AdoptionRequest
            {
                PetId = request.PetId,
                OwnerId = pet.OwnerId,
                AdopterId = adopterId,
                Message = request.Message,
                Status = AdoptionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AdoptionRequests.AddAsync(newRequest);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
