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

            var ownershipTypes = ownershipTypeEntities
                        .Select(o => OwnershipType.Create(o.Id, o.Name, o.Stores.Select(s => Store.Create(s.Id, s.StoreNumber, s.Name, s.YearOfOpened, s.Phone, s.OwnershipTypeId, s.DistrictId).Value),
                        o.Suppliers.Select(s => Supplier.Create(s.Id, s.Name, s.OwnershipTypeId, s.CityId, s.Phone).Value)).Value)
                .ToList();

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

            var ownershipType = OwnershipType.Create(ownershipTypesEntity.Id, ownershipTypesEntity.Name, ownershipTypesEntity.Stores.Select(s => Store.Create(s.Id, s.StoreNumber, s.Name, s.YearOfOpened, s.Phone, s.OwnershipTypeId, s.DistrictId).Value),
                        ownershipTypesEntity.Suppliers.Select(s => Supplier.Create(s.Id, s.Name, s.OwnershipTypeId, s.CityId, s.Phone).Value)).Value;

            return ownershipType;
        }

        public async Task Create(OwnershipType ownershipType)
        {
            var ownershipTypeEntity = new OwnershipTypeEntity
            {
                Name = ownershipType.Name,
            };

            await _context.AddAsync(ownershipTypeEntity);
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
    }
}
