using PetAdopt.DAL.Data;
using PetAdopt.DAL.Entities;
using PetAdopt.DAL.Reposetories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.DAL.Reposetories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<Pet> Pets { get; private set; }
        public IGenericRepository<AdoptionRequest> AdoptionRequests { get; private set; }
        public IGenericRepository<Favorite> Favorites { get; private set; }
        public IGenericRepository<Feedback> Feedbacks { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Pets = new GenericRepository<Pet>(_context);
            AdoptionRequests = new GenericRepository<AdoptionRequest>(_context);
            Favorites = new GenericRepository<Favorite>(_context);
            Feedbacks = new GenericRepository<Feedback>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}