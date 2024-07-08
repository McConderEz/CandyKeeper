using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Application.Services
{
    public class PackagingService : IPackagingService
    {
        private readonly IPackagingRepository _repository;

        public PackagingService(IPackagingRepository packagingRepository)
        {
            _repository = packagingRepository;
        }

        public async Task<List<Packaging>> Get()
        {
            return await _repository.Get();
        }

        public async Task<Packaging> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(Packaging packaging)
        {
            await _repository.Create(packaging);
        }

        public async Task Update(Packaging packaging)
        {
            await _repository.Update(packaging.Id, packaging.Name);
        }
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
