using CandyKeeper.Application.Interfaces;
using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _repository;

        public StoreService(IStoreRepository storeRepository)
        {
            _repository = storeRepository;
        }

        public async Task<List<Store>> GetBySearchingString(string searchingString)
        {
            var stores = await _repository.Get();
            return stores.Where(p => p.Name.Contains(searchingString)).ToList();
        }
        
        public async Task<List<Store>> Get()
        {
            return await _repository.Get();
        }

        public async Task<Store> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(Store store)
        {
            await _repository.Create(store);
        }

        public async Task Update(Store store)
        {
            await _repository.Update(store.Id, store.StoreNumber, store.Name, store.YearOfOpened, store.NumberOfEmployees,
                                     store.Phone, store.OwnershipTypeId, store.DistrictId);
        }

        public async Task AddSupplier(int id, Supplier model)
        {
            await _repository.AddSupplier(id, model);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public async Task DeleteSupplier(int id, Supplier model)
        {
            await _repository.DeleteSupplier(id, model);
        }
    }
}
