using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Repositories
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public DistrictRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<District>> Get()
        {
            await _semaphore.WaitAsync();
            try
            {
                var districtEntities = await _context.Districts
                    .AsNoTracking()
                    .Include(d => d.City)
                    .Include(d => d.Stores)
                    .ThenInclude(s => s.OwnershipType)
                    .ToListAsync();

                var districts = districtEntities.Select(d => MapToDistrict(d)).ToList();

                return districts;
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

        public async Task<District> GetById(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                var districtEntities = await _context.Districts
                    .Include(d => d.City)
                    .Include(d => d.Stores)
                    .ThenInclude(s => s.OwnershipType)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (districtEntities == null)
                    throw new Exception("DistrictEntity is null");

                var district = MapToDistrict(districtEntities);

                return district;
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

        public async Task Create(District district)
        {
            await _semaphore.WaitAsync();

            try
            {
                var districtEntity = MapToDistrictEntity(district);

                await _context.Districts.AddAsync(districtEntity);
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

        public async Task Update(int id, string name, int cityId)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.Districts
                    .Where(d => d.Id == id)
                    .ExecuteUpdateAsync(d => d
                        .SetProperty(d => d.Name, name)
                        .SetProperty(d => d.CityId, cityId));

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
                await _context.Districts
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

        private District MapToDistrict(DistrictEntity districtEntity)
        {
            var stores = districtEntity.Stores.Select(s => MapToStore(s)).ToList();

            return District.Create(
                districtEntity.Id,
                districtEntity.Name,
                districtEntity.CityId,
                MapToCity(districtEntity.City),
                stores
            ).Value;
        }

        private City MapToCity(CityEntity city)
        {
            return City.Create(city.Id, city.Name).Value;
        }

        private DistrictEntity MapToDistrictEntity(District district)
        {
            var stores = district.Stores.Select(s => MapToStoreEntity(s)).ToList();

            return new DistrictEntity
            {
                Id = district.Id,
                Name = district.Name,
                CityId = district.CityId,
                Stores = stores
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
                storeEntity.DistrictId,
                MapToOwnershipType(storeEntity.OwnershipType)
            ).Value;
        }

        private OwnershipType MapToOwnershipType(OwnershipTypeEntity ownershipType)
        {
            return OwnershipType.Create(ownershipType.Id, ownershipType.Name).Value;
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
    }
}
