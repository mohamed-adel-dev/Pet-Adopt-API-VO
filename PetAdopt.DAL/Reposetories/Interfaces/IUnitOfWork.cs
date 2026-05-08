using PetAdopt.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.DAL.Reposetories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Pet> Pets { get; }
        IGenericRepository<AdoptionRequest> AdoptionRequests { get; }
        IGenericRepository<Favorite> Favorites { get; }
        IGenericRepository<Feedback> Feedbacks { get; }

        Task<int> SaveChangesAsync();
    }
}
