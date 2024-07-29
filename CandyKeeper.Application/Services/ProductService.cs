using CandyKeeper.Application.Interfaces;
using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository productRepository)
        {
            _repository = productRepository;
        }
        
        public async Task<List<Product>> GetBySearchingString(string searchingString)
        {
            var products = await _repository.Get();
            return products.Where(p => p.Name.Contains(searchingString)).ToList();
        }

        public async Task<List<Product>> Get()
        {
            return await _repository.Get();
        }

        public async Task<Product> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(Product product)
        {
            await _repository.Create(product);
        }

        public async Task Update(Product product)
        {
            await _repository.Update(product.Id, product.Name, product.ProductTypeId);
        }
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
