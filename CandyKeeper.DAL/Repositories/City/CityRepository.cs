﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CandyKeeper.DAL.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public CityRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<City>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var cityEntities = await _context.Cities
                    .AsNoTracking()
                    .Include(c => c.Districts)
                    .Include(c => c.Suppliers)
                    .ThenInclude(s => s.OwnershipType)
                    .ToListAsync();

                var cities = cityEntities
                    .Select(c => MapToCity(c))
                    .ToList();

                return cities;
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

        public async Task<City> GetById(int id)
        {

            await _semaphore.WaitAsync();

            try
            {

                IQueryable<CityEntity> cityEntity = _context.Cities.Where(c => c.Id == id).AsNoTracking();

                if (cityEntity == null)
                    throw new Exception("CityEntity is null");

                var city = MapToCity((await cityEntity.Include(c => c.Districts)
                    .Include(c => c.Suppliers)
                    .ThenInclude(s => s.OwnershipType)
                    .SingleOrDefaultAsync())!);
                return city;
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

        public async Task Create(City city)
        {
            await _semaphore.WaitAsync();

            try
            {
                var cityEntity = MapToCityEntity(city);

                await _context.Cities.AddAsync(cityEntity);
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
                await _context.Cities
                    .Where(c => c.Id == id)
                    .ExecuteUpdateAsync(c => c
                        .SetProperty(c => c.Name, name));

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
                await _context.Cities
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

        private City MapToCity(CityEntity cityEntity)
        {
            var districts = cityEntity.Districts.Select(d => MapToDistrict(d)).ToList();
            var suppliers = cityEntity.Suppliers.Select(s => MapToSupplier(s)).ToList();

            return City.Create(
                cityEntity.Id,
                cityEntity.Name,
                districts,
                suppliers
            ).Value;
        }

        private OwnershipType MapToOwnershipType(OwnershipTypeEntity ownershipTypeEntity)
        {
            return OwnershipType.Create(
                ownershipTypeEntity.Id,
                ownershipTypeEntity.Name
            ).Value;
        }

        private CityEntity MapToCityEntity(City city)
        {
            var districts = city.Districts.Select(d => MapToDistrictEntity(d)).ToList();
            var suppliers = city.Suppliers.Select(s => MapToSupplierEntity(s)).ToList();

            return new CityEntity
            {
                Id = city.Id,
                Name = city.Name,
                Districts = districts,
                Suppliers = suppliers
            };
        }

        private District MapToDistrict(DistrictEntity districtEntity)
        {
            return District.Create(
                districtEntity.Id,
                districtEntity.Name,
                districtEntity.CityId
            ).Value;
        }

        private DistrictEntity MapToDistrictEntity(District district)
        {
            return new DistrictEntity
            {
                Id = district.Id,
                Name = district.Name,
                CityId = district.CityId
            };
        }

        private Supplier MapToSupplier(SupplierEntity supplierEntity)
        {
            return Supplier.Create(
                supplierEntity.Id,
                supplierEntity.Name,
                supplierEntity.OwnershipTypeId,
                supplierEntity.CityId,
                supplierEntity.Phone,
                MapToOwnershipType(supplierEntity.OwnershipType)
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
