using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Repositories
{
    public class OwnershipTypeRepository : IOwnershipTypeRepository
    {
        private readonly CandyKeeperDbContext _context;

        public OwnershipTypeRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<OwnershipType>> Get()
        {
            var ownershipTypeEntities = await _context.OwnershipTypes
                .AsNoTracking()
                .Include(o => o.Stores)
                .Include(o => o.Suppliers)
                .ToListAsync();

            var ownershipTypes = ownershipTypeEntities.Select(o => MapToOwnershipType(o)).ToList();

            return ownershipTypes;
        }

        public async Task<OwnershipType> GetById(int id)
        {
            var ownershipTypesEntity = await _context.OwnershipTypes
                                           .Include(o => o.Suppliers)
                                           .Include(o => o.Stores)
                                           .FirstOrDefaultAsync(c => c.Id == id);

            if (ownershipTypesEntity == null)
                throw new Exception("OwnershipType is null");

            var ownershipType = MapToOwnershipType(ownershipTypesEntity);

            return ownershipType;
        }

        public async Task Create(OwnershipType ownershipType)
        {
            var ownershipTypeEntity = MapToOwnershipTypeEntity(ownershipType);  

            await _context.OwnershipTypes.AddAsync(ownershipTypeEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, string name)
        {
            await _context.OwnershipTypes
                .Where(d => d.Id == id)
                .ExecuteUpdateAsync(d => d
                                   .SetProperty(d => d.Name, name));

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.OwnershipTypes
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
        }

        private OwnershipType MapToOwnershipType(OwnershipTypeEntity ownershipTypeEntity)
        {
            var stores = ownershipTypeEntity.Stores.Select(s => MapToStore(s)).ToList();
            var suppliers = ownershipTypeEntity.Suppliers.Select(s => MapToSupplier(s)).ToList();

            return OwnershipType.Create(
                ownershipTypeEntity.Id,
                ownershipTypeEntity.Name,
                stores,
                suppliers
            ).Value;
        }

        private OwnershipTypeEntity MapToOwnershipTypeEntity(OwnershipType ownershipType)
        {
            var stores = ownershipType.Stores.Select(s => MapToStoreEntity(s)).ToList();
            var suppliers = ownershipType.Suppliers.Select(s => MapToSupplierEntity(s)).ToList();

            return new OwnershipTypeEntity
            {
                Id = ownershipType.Id,
                Name = ownershipType.Name,
                Stores = stores,
                Suppliers = suppliers
            };
        }

        private Store MapToStore(StoreEntity storeEntity)
        {
            return Store.Create(
                storeEntity.Id,
                storeEntity.StoreNumber,
                storeEntity.Name,
                storeEntity.YearOfOpened,
                storeEntity.Phone,
                storeEntity.OwnershipTypeId,
                storeEntity.DistrictId
            ).Value;
        }

        private StoreEntity MapToStoreEntity(Store store)
        {
            return new StoreEntity
            {
                Id = store.Id,
                StoreNumber = store.StoreNumber,
                Name = store.Name,
                YearOfOpened = store.YearOfOpened,
                Phone = store.Phone,
                OwnershipTypeId = store.OwnershipTypeId,
                DistrictId = store.DistrictId
            };
        }

        private Supplier MapToSupplier(SupplierEntity supplierEntity)
        {
            return Supplier.Create(
                supplierEntity.Id,
                supplierEntity.Name,
                supplierEntity.OwnershipTypeId,
                supplierEntity.CityId,
                supplierEntity.Phone
            ).Value;
        }

        private SupplierEntity MapToSupplierEntity(Supplier supplier)
        {
            return new SupplierEntity
            {
                Id = supplier.Id,
                Name = supplier.Name,
                OwnershipTypeId = supplier.OwnershipTypeId,
                CityId = supplier.CityId,
                Phone = supplier.Phone
            };
        }
    }
}
