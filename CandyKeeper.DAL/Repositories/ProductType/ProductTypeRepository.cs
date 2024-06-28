using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Repositories
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

            var productTypes = productTypeEntities
                .Select(p => ProductType.Create(p.Id, p.Name, p.Products.Select(p => Product.Create(p.Id, p.Name, p.ProductTypeId, p.PackagingId).Value)).Value)
                .ToList();

            return productTypes;
        }

        public async Task<ProductType> GetById(int id)
        {
            var productTypeEntity = await _context.ProductTypes
                                           .Include(p => p.Products)
                                           .FirstOrDefaultAsync(c => c.Id == id);

            if (productTypeEntity == null)
                throw new Exception("productType null");


            var productType = ProductType.Create(productTypeEntity.Id, productTypeEntity.Name, productTypeEntity.Products.Select(p => Product.Create(p.Id, p.Name, p.ProductTypeId, p.PackagingId).Value)).Value;

            return productType;
        }

        public async Task Create(ProductType productType)
        {
            var productTypeEntity = new ProductTypeEntity
            {
                Name = productType.Name,
            };

            await _context.AddAsync(productTypeEntity);
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
    }
}
