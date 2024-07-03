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
    public class ProductForSaleService : IProductForSaleService
    {
        private readonly IProductForSaleRepository _repository;

        public ProductForSaleService(IProductForSaleRepository productForSaleRepository)
        {
            _repository = productForSaleRepository;
        }

        public async Task<List<ProductForSale>> Get()
        {
            return await _repository.Get();
        }

        public async Task<ProductForSale> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(ProductForSale productForSale)
        {
            await _repository.Create(productForSale);
        }

        public async Task Update(ProductForSale productForSale)
        {
            await _repository.Update(productForSale.Id, productForSale.Price, productForSale.Volume, productForSale.ProductId,
                                    productForSale.StoreId, productForSale.ProductDeliveryId, productForSale.PackagingId);
        }
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
