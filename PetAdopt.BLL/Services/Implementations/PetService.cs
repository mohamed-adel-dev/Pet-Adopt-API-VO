using Microsoft.EntityFrameworkCore;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.DAL.Entities;
using PetAdopt.DAL.Entities.Enums;
using PetAdopt.DAL.Reposetories.Interfaces;

namespace PetAdopt.BLL.Services.Implementations
{
    public class PetService : IPetService
    {

        // Dependency injection of the unit of work for database operations
        private readonly IUnitOfWork _unitOfWork;
        public PetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ApprovePetAsync(int petId)
        {
            // Validate the petId parameter
            if (petId <= 0)
                throw new ArgumentException("Invalid petId");

            // Retrieve the pet from the database using the unit of work
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null)
                throw new Exception("Pet not found");

            // Check if the pet is in the pending approval status
            if (pet.PostStatus != PostStatus.PendingApproval)
                throw new Exception("Already processed");


            pet.PostStatus = PostStatus.Approved;
            pet.Status = PetStatus.Available;

            await _unitOfWork.Pets.UpdateAsync(pet);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<PetDto?> CreateAsync(string ownerId, string ownerName, CreatePetDto dto)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException("Owner ID is required.");

            if (string.IsNullOrEmpty(ownerName))
                throw new ArgumentException("Owner name is required."); 

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Create a new pet entity based on the provided DTO and owner ID
            var pet = new Pet
            {
                OwnerId = ownerId,
                OwnerName = ownerName,
                PetName = dto.PetName,
                AnimalType = dto.AnimalType,
                Age = dto.Age,
                Breed = dto.Breed,
                Gender = dto.Gender,
                HealthStatus = dto.HealthStatus,
                Description = dto.Description,
                Location = dto.Location,
                ImageUrl = dto.ImageUrl,

                Status = PetStatus.Available,
                PostStatus = PostStatus.PendingApproval,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Pets.AddAsync(pet);
            await _unitOfWork.SaveChangesAsync();

            // Return a DTO representing the newly created pet
            return new PetDto
            {
                Id = pet.Id,
                OwnerId = ownerId,
                OwnerName = pet.OwnerName,
                PetName = pet.PetName,
                AnimalType = pet.AnimalType,
                Age = pet.Age,
                Breed = pet.Breed,
                Location = pet.Location,
                ImageUrl = pet.ImageUrl,
                Status = pet.Status.ToString()
            };
        }

        public async Task<bool> DeleteAsync(int petId, string ownerId)
        {
            // Validate input parameters
            if (petId <= 0)
                throw new ArgumentException("Invalid petId");

            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException("Invalid ownerId");

            // Get the pet from the database
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);

            if (pet == null)
                return false;

            // Check if the current user is the owner of the pet
            if (pet.OwnerId != ownerId)
                throw new UnauthorizedAccessException("You are not the owner of this pet");

            await _unitOfWork.Pets.DeleteAsync(pet.Id);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<PetDto>> GetAllApprovedAsync()
        {
            // Retrieve all pets with the approved post status from the database
            var pets = await _unitOfWork.Pets
                .Query()
                .Where(p => p.PostStatus == PostStatus.Approved)
                    .ToListAsync();

            // Map the retrieved pet entities to a list of PetDto objects and return it
            return pets.Select(p => new PetDto
            {
                Id = p.Id,
                PetName = p.PetName,
                AnimalType = p.AnimalType,
                Age = p.Age,
                Breed = p.Breed,
                Location = p.Location,
                ImageUrl = p.ImageUrl,
                Status = p.Status.ToString()
            }).ToList();

        }

        public async Task<PetDto?> GetByIdAsync(int petId)
        {
            // Validate the petId parameter
            if (petId <= 0)
                throw new ArgumentException("Invalid petId");

            // Retrieve the pet from the database using the unit of work
            var result = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (result == null)
                throw new ArgumentException("Pet not found");

            // Map the retrieved pet entity to a PetDto object and return it
            return new PetDto
            {
                Id = result.Id,
                PetName = result.PetName,
                AnimalType = result.AnimalType,
                Age = result.Age,
                Breed = result.Breed,
                Location = result.Location,
                ImageUrl = result.ImageUrl,
                Status = result.Status.ToString()
            };

        }

        public async Task<List<PetDto>> GetByOwnerAsync(string ownerId)
        {
            // Validate the ownerId parameter
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException("Invalid ownerId");

            // Retrieve all pets that belong to the specified owner from the database
            var pets = await _unitOfWork.Pets
                .Query()
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();

            // Map the retrieved pet entities to a list of PetDto objects and return it
            return pets.Select(p => new PetDto
            {
                Id = p.Id,
                PetName = p.PetName,
                AnimalType = p.AnimalType,
                Age = p.Age,
                Breed = p.Breed,
                Location = p.Location,
                ImageUrl = p.ImageUrl,
                Status = p.Status.ToString()
            }).ToList();

        }

        public async Task RejectPetAsync(int petId)
        {
            // Validate the petId parameter
            if (petId <= 0)
                throw new ArgumentException("Invalid petId");

            // Retrieve the pet from the database using the unit of work
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null)
                throw new Exception("Pet not found");

            pet.PostStatus = PostStatus.Rejected;
            pet.Status = PetStatus.Available;


            await _unitOfWork.Pets.UpdateAsync(pet);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<PetDto>> SearchAsync(PetSearchDto filter)
        {
            // Start with a query that retrieves all approved pets from the database
            var query = _unitOfWork.Pets.Query()
                .Where(p => p.PostStatus == PostStatus.Approved);

            // Apply filters to the query based on the properties of the PetSearchDto object
            if (!string.IsNullOrEmpty(filter.AnimalType))
                query = query.Where(p => p.AnimalType == filter.AnimalType);

            if (!string.IsNullOrEmpty(filter.Breed))
                query = query.Where(p => p.Breed == filter.Breed);

            if (filter.Age.HasValue)
                query = query.Where(p => p.Age == filter.Age.Value);

            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(p => p.Location.Contains(filter.Location));

            // Execute the query and retrieve the matching pets from the database
            var pets = await query.ToListAsync();

            return pets.Select(p => new PetDto
            {
                Id = p.Id,
                PetName = p.PetName,
                AnimalType = p.AnimalType,
                Age = p.Age,
                Breed = p.Breed,
                Location = p.Location,
                ImageUrl = p.ImageUrl,
                Status = p.Status.ToString()
            }).ToList();
        }

        public async Task<PetDto> UpdateAsync(int petId, string ownerId, UpdatePetDto dto)
        {
            // Validate the petId parameter
            if (petId <= 0)
                throw new ArgumentException("Invalid petId");

            // Validate the ownerId parameter
            if (string.IsNullOrEmpty(ownerId))
                throw new ArgumentException("Invalid ownerId");

            // Validate the dto parameter
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null)
                throw new Exception("Pet not found");

            // Check if the current user is the owner of the pet
            if (pet.OwnerId != ownerId)
                throw new UnauthorizedAccessException("You are not the owner of this pet");

            // Update the pet's properties based on the provided DTO
            pet.PetName = dto.PetName;
            pet.AnimalType = dto.AnimalType;
            pet.Age = dto.Age;
            pet.Breed = dto.Breed;
            pet.Location = dto.Location;
            pet.Description = dto.Description;

            await _unitOfWork.Pets.UpdateAsync(pet);
            await _unitOfWork.SaveChangesAsync();

            // Map the updated pet entity to a PetDto object and return it
            return new PetDto
            {
                Id = pet.Id,
                PetName = pet.PetName,
                AnimalType = pet.AnimalType,
                Age = pet.Age,
                Breed = pet.Breed,
                Location = pet.Location,
                ImageUrl = pet.ImageUrl,
                Status = pet.Status.ToString()
            };
        }
    }
}
