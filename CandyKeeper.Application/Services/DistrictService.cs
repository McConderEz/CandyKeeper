using CandyKeeper.Application.Interfaces;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Application.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _repository;

        public DistrictService(IDistrictRepository districtRepository)
        {
            _repository = districtRepository;
        }

        public async Task<List<District>> GetBySearchingString(string searchingString)
        {
            var districts = await _repository.Get();
            return districts.Where(d => d.Name == searchingString || d.City.Name == searchingString).ToList();
        }
        
        public async Task<List<District>> Get()
        {
            return await _repository.Get();
        }

        public async Task<District> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(District district)
        {
            await _repository.Create(district);
        }

        public async Task Update(District district)
        {
            await _repository.Update(district.Id, district.Name, district.CityId);
        }
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
