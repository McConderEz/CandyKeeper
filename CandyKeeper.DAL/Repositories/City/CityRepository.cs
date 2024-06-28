using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CandyKeeper.DAL.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly CandyKeeperDbContext _context;

        public CityRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<City>> Get()
        {
            var cityEntities = await _context.Cities
                .AsNoTracking()
                .Include(c => c.Districts)
                .ToListAsync();

            var cities = cityEntities
                .Select(c => City.Create(c.Id, c.Name, c.Districts.Select(d => District.Create(d.Id, d.Name, d.CityId).Value)).Value)
                .ToList();

            return cities;
        }

        public async Task<City> GetById(int id)
        {
            var cityEntity = await _context.Cities
                                           .Include(c => c.Districts)
                                           .FirstOrDefaultAsync(c => c.Id == id);

            var city = City.Create(cityEntity.Id, cityEntity.Name, cityEntity.Districts.Select(d => District.Create(d.Id, d.Name, d.CityId).Value)).Value;

            return city;
        }

        public async Task Create(City city)
        {
            var cityEntity = new CityEntity
            {
                Name = city.Name,
                Districts = city.Districts.Select(d => new DistrictEntity { Id = d.Id, Name = d.Name }).ToList(),
            };

            await _context.AddAsync(cityEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, string name)
        {
            await _context.Cities
                .Where(c => c.Id == id)
                .ExecuteUpdateAsync(c => c
                                   .SetProperty(c => c.Name, name));

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Cities
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
        }
    }
}
