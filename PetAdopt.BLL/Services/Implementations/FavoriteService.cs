using Microsoft.EntityFrameworkCore;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;
using PetAdopt.DAL.Entities;
using PetAdopt.DAL.Reposetories.Interfaces;

namespace PetAdopt.BLL.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        // Dependency injection of the unit of work for database operations
        private readonly IUnitOfWork _unitOfWork;
        public FavoriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddAsync(string adopterId, int petId)
        {
            // Validate input parameters
            if (petId <= 0)
                throw new ArgumentException("Invalid pet ID.");

            if (string.IsNullOrEmpty(adopterId))
                throw new ArgumentException("Adopter ID is required.");

            // Check if the pet exists in the database
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null)
                throw new InvalidOperationException("Pet not found");

            // Check if the pet is already added to favorites
            var existingFavorite = await _unitOfWork.Favorites
                .Query()
                .FirstOrDefaultAsync(f =>
                f.PetId == petId &&
                f.AdopterId == adopterId);

            if (existingFavorite != null)
                throw new InvalidOperationException("This pet is already in favorites.");

            // Create a new favorite entry
            var favorite = new Favorite
            {
                AdopterId = adopterId,
                PetId = petId
            };

            await _unitOfWork.Favorites.AddAsync(favorite);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<PetDto>> GetFavoritesAsync(string adopterId)
        {
            // Validate input parameter
            if (string.IsNullOrEmpty(adopterId))
                throw new ArgumentException("Adopter ID is required.");

            // Retrieve the list of favorite pets for the given adopter
            var favorites = await _unitOfWork.Favorites
                .Query()
                .Where(f => f.AdopterId == adopterId)
                .Select(f => f.Pet)
                .ToListAsync();

            // Map the list of Pet entities to a list of PetDto objects
            return favorites.Select(p => new PetDto
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

        public async Task<bool> RemoveAsync(string adopterId, int petId)
        {

            // Validate input parameters
            if (petId <= 0)
                throw new ArgumentException("Invalid pet ID.");

            if (string.IsNullOrEmpty(adopterId))
                throw new ArgumentException("Adopter ID is required.");

            // Check if the favorite entry exists for the given adopter and pet
            var existingFavorite = await _unitOfWork.Favorites
                .Query()
                .FirstOrDefaultAsync(f =>
                    f.PetId == petId &&
                    f.AdopterId == adopterId);

            if (existingFavorite == null)
                return false;

           
            var result = await _unitOfWork.Favorites.DeleteAsync(existingFavorite.Id);

            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
