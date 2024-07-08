using CandyKeeper.Application.Interfaces;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Application.Services
{
    public class OwnershipTypeService : IOwnershipTypeService
    {
        private readonly IOwnershipTypeRepository _repository;

        public OwnershipTypeService(IOwnershipTypeRepository ownershipTypeRepository)
        {
            _repository = ownershipTypeRepository;
        }

        public async Task<List<OwnershipType>> Get()
        {
            return await _repository.Get();
        }

        public async Task<OwnershipType> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(OwnershipType ownershipType)
        {
            await _repository.Create(ownershipType);
        }

        public async Task Update(OwnershipType ownershipType)
        {
            await _repository.Update(ownershipType.Id, ownershipType.Name);
        }
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
