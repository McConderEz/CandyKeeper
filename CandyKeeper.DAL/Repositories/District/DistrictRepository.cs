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
    public class DistrictRepository
    {
        private readonly CandyKeeperDbContext _context;

        public DistrictRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<District>> Get()
        {
            var districtEntities = await _context.Districts
                .AsNoTracking()
                .Include(d => d.City)
                .Include(d => d.Stores)
                .ToListAsync();

            var districts = districtEntities
                .Select(d => District.Create(d.Id, d.Name, d.CityId, d.Stores.Select(s => Store.Create(s.Id, s.StoreNumber, s.Name, s.YearOfOpened, s.Phone,
                                                                     s.OwnershipTypeId, s.DistrictId).Value)).Value)
                .ToList();

            return districts;
        }

        public async Task<District> GetById(int id)
        {
            var districtEntities = await _context.Districts
                                           .Include(d => d.City)
                                           .Include(d => d.Stores)
                                           .FirstOrDefaultAsync(c => c.Id == id);

            var district = District.Create(districtEntities.Id, districtEntities.Name, districtEntities.CityId, districtEntities.Stores.Select(s => Store.Create(s.Id, s.StoreNumber, s.Name, s.YearOfOpened, s.Phone,
                                                                     s.OwnershipTypeId, s.DistrictId).Value)).Value;

            return district;
        }

        public async Task Create(District district)
        {
            var districtEntity = new DistrictEntity
            {
                Name = district.Name,
                CityId = district.CityId
            };

            await _context.AddAsync(districtEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, string name, int cityId)
        {
            await _context.Districts
                .Where(d => d.Id == id)
                .ExecuteUpdateAsync(d => d
                                   .SetProperty(d => d.Name, name)
                                   .SetProperty(d => d.CityId, cityId));

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Districts
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
        }
    }
}
