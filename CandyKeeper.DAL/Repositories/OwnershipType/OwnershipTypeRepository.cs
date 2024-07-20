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
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public OwnershipTypeRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<OwnershipType>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var ownershipTypeEntities = await _context.OwnershipTypes
                    .AsNoTracking()
                    .Include(o => o.Stores)
                    .Include(o => o.Suppliers)
                    .ToListAsync();

                var ownershipTypes = ownershipTypeEntities.Select(o => MapToOwnershipType(o)).ToList();

                return ownershipTypes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<OwnershipType> GetById(int id)
        {
            await _semaphore.WaitAsync();


            try
            {
                IQueryable<OwnershipTypeEntity> ownershipTypesEntity = _context.OwnershipTypes.Where(o => o.Id == id).AsNoTracking();

                if (ownershipTypesEntity == null)
                    throw new Exception("OwnershipType is null");

                var ownershipType = MapToOwnershipType((await ownershipTypesEntity.Include(o => o.Suppliers)
                    .Include(o => o.Stores)
                    .ThenInclude(s => s.District)
                    .ThenInclude(d => d.City)
                    .SingleOrDefaultAsync())!);

                return ownershipType;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Create(OwnershipType ownershipType)
        {
            await _semaphore.WaitAsync();

            try
            {
                var ownershipTypeEntity = MapToOwnershipTypeEntity(ownershipType);

                await _context.OwnershipTypes.AddAsync(ownershipTypeEntity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Update(int id, string name)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.OwnershipTypes
                    .Where(d => d.Id == id)
                    .ExecuteUpdateAsync(d => d
                        .SetProperty(d => d.Name, name));

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Delete(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.OwnershipTypes
                    .Where(c => c.Id == id)
                    .ExecuteDeleteAsync();

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
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
            var district = storeEntity.District != null ? MapToDistrict(storeEntity.District) : null;
            return Store.Create(
                storeEntity.Id,
                storeEntity.StoreNumber,
                storeEntity.Name,
                storeEntity.YearOfOpened,
                storeEntity.Phone,
                storeEntity.OwnershipTypeId,
                storeEntity.DistrictId,
                null,
                district
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

        private District MapToDistrict(DistrictEntity? storeDistrict)
        {
            return District.Create(storeDistrict.Id, storeDistrict.Name, storeDistrict.CityId,
                MapToCity(storeDistrict.City)).Value;
        }

        private City MapToCity(CityEntity? storeDistrictCity)
        {
            return City.Create(storeDistrictCity.Id, storeDistrictCity.Name).Value;
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
