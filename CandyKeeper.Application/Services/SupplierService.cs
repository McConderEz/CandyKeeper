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
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _repository = supplierRepository;
        }

        public async Task<List<Supplier>> Get()
        {
            return await _repository.Get();
        }

        public async Task<Supplier> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(Supplier supplier)
        {
            await _repository.Create(supplier);
        }

        public async Task Update(Supplier supplier)
        {
            await _repository.Update(supplier.Id, supplier.Name, supplier.Phone, supplier.OwnershipTypeId, supplier.CityId);
        }

        public async Task AddStore(int id, Store model)
        {
            await _repository.AddStore(id, model);
        }

        public async Task DeleteStore(int id, Store model)
        {
            await _repository.DeleteStore(id, model);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}

