using CandyKeeper.DAL.Entities;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL
{
    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly CandyKeeperDbContext _context;

        public ProductTypeRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductType>> Get()
        {
            var productTypeEntities = await _context.ProductTypes
                .AsNoTracking()
                .Include(p => p.Products)
                .ToListAsync();

            var productTypes = productTypeEntities.Select(pt => MapToProductType(pt)).ToList();
                

            return productTypes;
        }

        public async Task<ProductType> GetById(int id)
        {
            var productTypeEntity = await _context.ProductTypes
                                           .Include(p => p.Products)
                                           .FirstOrDefaultAsync(c => c.Id == id);

            if (productTypeEntity == null)
                throw new Exception("productType null");


            var productType = MapToProductType(productTypeEntity);

            return productType;
        }

        public async Task Create(ProductType productType)
        {
            var productTypeEntity = MapToProductTypeEntity(productType);

            await _context.ProductTypes.AddAsync(productTypeEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, string name)
        {
            await _context.ProductTypes
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p
                                   .SetProperty(p => p.Name, name));

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.ProductTypes
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
        }

        private ProductType MapToProductType(ProductTypeEntity productTypeEntity)
        {
            var products = productTypeEntity.Products.Select(p => MapToProduct(p)).ToList();

            return ProductType.Create(
                productTypeEntity.Id,
                productTypeEntity.Name,
                products
            ).Value;
        }

        private ProductTypeEntity MapToProductTypeEntity(ProductType productType)
        {
            var products = productType.Products.Select(p => MapToProductEntity(p)).ToList();

            return new ProductTypeEntity
            {
                Id = productType.Id,
                Name = productType.Name,
                Products = products
            };
        }

        private Product MapToProduct(ProductEntity productEntity)
        {
            return Product.Create(
                productEntity.Id,
                productEntity.Name,
                productEntity.ProductTypeId
            ).Value;
        }

        private ProductEntity MapToProductEntity(Product product)
        {
            return new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                ProductTypeId = product.ProductTypeId
            };
        }
    }
}
