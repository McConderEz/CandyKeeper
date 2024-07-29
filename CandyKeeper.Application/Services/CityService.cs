using CandyKeeper.Application.Interfaces;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Application.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _repository;

        public CityService(ICityRepository cityRepository)
        {
            _repository = cityRepository;
        }

        public async Task<List<City>> GetBySearchingString(string searchingString)
        {
            var cities = await _repository.Get();
            return cities.Where(c => c.Name.Contains(searchingString)).ToList();
        }
        
        public async Task<List<City>> Get()
        {
            return await _repository.Get();
        }

        public async Task<City> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(City city)
        {
            await _repository.Create(city);
        }

        public async Task Update(City city)
        {
            await _repository.Update(city.Id, city.Name);
        }
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
