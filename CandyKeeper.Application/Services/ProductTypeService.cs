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
    public class ProductTypeService : IProductTypeService
    {
        private readonly IProductTypeRepository _repository;

        public ProductTypeService(IProductTypeRepository productTypeRepository)
        {
            _repository = productTypeRepository;
        }
        
        public async Task<List<ProductType>> GetBySearchingString(string searchingString)
        {
            var productTypes = await _repository.Get();
            return productTypes.Where(p => p.Name.Contains(searchingString)).ToList();
        }

        public async Task<List<ProductType>> Get()
        {
            return await _repository.Get();
        }

        public async Task<ProductType> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(ProductType productType)
        {
            await _repository.Create(productType);
        }

        public async Task Update(ProductType productType)
        {
            await _repository.Update(productType.Id, productType.Name);
        }
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
