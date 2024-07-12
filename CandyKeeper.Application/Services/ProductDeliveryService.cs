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
    public class ProductDeliveryService : IProductDeliveryService
    {
        private readonly IProductDeliveryRepository _repository;

        public ProductDeliveryService(IProductDeliveryRepository productDeliveryRepository)
        {
            _repository = productDeliveryRepository;
        }

        public async Task<List<ProductDelivery>> Get()
        {
            return await _repository.Get();
        }

        public async Task<ProductDelivery> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(ProductDelivery productDelivery)
        {
            await _repository.Create(productDelivery);
        }

        public async Task Update(ProductDelivery productDelivery)
        {
            await _repository.Update(productDelivery.Id, productDelivery.DeliveryDate, productDelivery.SupplierId, productDelivery.StoreId);
        }

        public async Task AddProductForSale(int id, ProductForSale model)
        {
            await _repository.AddProductForSale(id, model);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
