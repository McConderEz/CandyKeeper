using CandyKeeper.DAL.Entities;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace CandyKeeper.DAL
{
    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);

        public ProductTypeRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductType>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var productTypeEntities = await _context.ProductTypes
                    .AsNoTracking()
                    .Include(p => p.Products)
                    .ToListAsync();

                var productTypes = productTypeEntities.Select(pt => MapToProductType(pt)).ToList();


                return productTypes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<ProductType> GetById(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                var productTypeEntity = await _context.ProductTypes
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (productTypeEntity == null)
                    throw new Exception("productType null");


                var productType = MapToProductType(productTypeEntity);

                return productType;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Create(ProductType productType)
        {
            await _semaphore.WaitAsync();

            try
            {
                var productTypeEntity = MapToProductTypeEntity(productType);

                await _context.ProductTypes.AddAsync(productTypeEntity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Update(int id, string name)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.ProductTypes
                    .Where(p => p.Id == id)
                    .ExecuteUpdateAsync(p => p
                        .SetProperty(p => p.Name, name));

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Delete(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.ProductTypes
                    .Where(p => p.Id == id)
                    .ExecuteDeleteAsync();

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
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
