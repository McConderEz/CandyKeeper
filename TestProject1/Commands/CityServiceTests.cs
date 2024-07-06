using CandyKeeper.Application.Services;
using CandyKeeper.DAL.Entities;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Tests.Commands
{
    public class CityServiceTests
    {
        private readonly DbContextOptions<TestDbContext> _options;
        private readonly TestDbContext _context;
        private readonly CityRepository _reposity;
        private readonly CityService _cityService;

        public CityServiceTests()
        {
            _options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseCandyKeeper")
                .Options;
            _context = new TestDbContext(_options);
            //_reposity = new CityRepository(_context);
            _cityService = new CityService(_reposity);
                
                
        }

        [Fact]
        public async Task Get_ShouldReturnAllCities()
        {
            // Arrange
            _context.Cities.AddRange(new List<CityEntity>
        {
            new CityEntity { Id = 0, Name = "City1" },
            new CityEntity { Id = 0, Name = "City2" }
        });
            await _context.SaveChangesAsync();

            // Act
            var result = await _cityService.Get();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnCity_WhenCityExists()
        {
            // Arrange
            var cityEntity = new CityEntity { Id = 1, Name = "City1" };
            _context.Cities.Add(cityEntity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cityService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("City1", result.Name);
        }

        [Fact]
        public async Task GetById_ShouldThrowException_WhenCityDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _cityService.GetById(999));
        }

        [Fact]
        public async Task Create_ShouldAddCity()
        {
            // Arrange
            var city = City.Create(0, "New City").Value;

            // Act
            await _cityService.Create(city);
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.Name == "New City");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Update_ShouldUpdateCityName()
        {
            // Arrange
            var cityEntity = new CityEntity { Id = 1, Name = "Old Name" };
            _context.Cities.Add(cityEntity);
            await _context.SaveChangesAsync();

            // Act
            await _cityService.Update(City.Create(1, "New Name").Value);
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.Id == 1);

            // Assert
            Assert.Equal("New Name", result.Name);
        }

        [Fact]
        public async Task Delete_ShouldRemoveCity()
        {
            // Arrange
            var cityEntity = new CityEntity { Id = 1, Name = "City1" };
            _context.Cities.Add(cityEntity);
            await _context.SaveChangesAsync();

            // Act
            await _cityService.Delete(1);
            var result = await _context.Cities.FirstOrDefaultAsync(c => c.Id == 1);

            // Assert
            Assert.Null(result);
        }
    }
}
