using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL
{
    public class ProductRepository : IProductRepository
    {
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public ProductRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var productEntities = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.ProductType)
                    .Include(p => p.ProductForSales)
                    .ThenInclude(pfs => pfs.Packaging)
                    .Include(p => p.ProductForSales)
                    .ThenInclude(pfs => pfs.Store)
                    .ToListAsync();

                var products = productEntities.Select(p => MapToProduct(p)).ToList();


                return products;
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

        public async Task<Product> GetById(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                IQueryable<ProductEntity> productEntity = _context.Products.Where(p => p.Id == id).AsNoTracking();
                
                if (productEntity == null)
                    throw new Exception("productType null");


                var product = MapToProduct((await productEntity
                    .Include(p => p.ProductType)
                    .Include(p => p.ProductForSales)
                    .ThenInclude(pfs => pfs.Packaging)
                    .Include(p => p.ProductForSales)
                    .ThenInclude(pfs => pfs.Store)
                    .SingleOrDefaultAsync())!);

                return product;
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

        public async Task Create(Product product)
        {
            await _semaphore.WaitAsync();

            try
            {
                var productEntity = MapToProductEntity(product);

                await _context.Products.AddAsync(productEntity);
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

        public async Task Update(int id, string name, int productTypeId)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.Products
                    .Where(p => p.Id == id)
                    .ExecuteUpdateAsync(p => p
                        .SetProperty(p => p.Name, name)
                        .SetProperty(p => p.ProductTypeId, productTypeId));

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

        private Product MapToProduct(ProductEntity productEntity)
        {
            var productType = productEntity.ProductType != null ? MapToProductType(productEntity.ProductType) : null;
            var productForSales = productEntity.ProductForSales.Select(pfs => MapToProductForSale(pfs)).ToList();

            return Product.Create(
                productEntity.Id,
                productEntity.Name,
                productEntity.ProductTypeId,
                productType,
                productForSales
            ).Value;
        }

        private ProductEntity MapToProductEntity(Product product)
        {
            var productTypeEntity = product.ProductType != null ? MapToProductTypeEntity(product.ProductType) : null;
            var productForSalesEntities = product.ProductForSales.Select(pfs => MapToProductForSaleEntity(pfs)).ToList();

            return new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                ProductTypeId = product.ProductTypeId,
                ProductType = productTypeEntity,
                ProductForSales = productForSalesEntities
            };
        }

        private ProductForSale MapToProductForSale(ProductForSaleEntity productForSaleEntity)
        {
            var store = productForSaleEntity.Store != null ? MapToStore(productForSaleEntity.Store) : null;
            var productDelivery = productForSaleEntity.ProductDelivery != null ? MapToProductDelivery(productForSaleEntity.ProductDelivery) : null;
            var packaging = productForSaleEntity.Packaging != null ? MapToPackaging(productForSaleEntity.Packaging) : null;

            return ProductForSale.Create(
                productForSaleEntity.Id,
                productForSaleEntity.ProductId,
                productForSaleEntity.StoreId,
                productForSaleEntity.ProductDeliveryId,
                productForSaleEntity.PackagingId,
                productForSaleEntity.Price,
                productForSaleEntity.Volume,
                null,
                store,
                productDelivery,
                packaging
            ).Value;
        }

        private ProductForSaleEntity MapToProductForSaleEntity(ProductForSale productForSale)
        {
            var productEntity = productForSale.Product != null ? MapToProductEntity(productForSale.Product) : null;
            var storeEntity = productForSale.Store != null ? MapToStoreEntity(productForSale.Store) : null;
            var productDeliveryEntity = productForSale.ProductDelivery != null ? MapToProductDeliveryEntity(productForSale.ProductDelivery) : null;
            var packagingEntity = productForSale.Packaging != null ? MapToPackagingEntity(productForSale.Packaging) : null;

            return new ProductForSaleEntity
            {
                Id = productForSale.Id,
                ProductId = productForSale.ProductId,
                StoreId = productForSale.StoreId,
                ProductDeliveryId = productForSale.ProductDeliveryId,
                PackagingId = productForSale.PackagingId,
                Price = productForSale.Price,
                Volume = productForSale.Volume,
                Product = productEntity,
                Store = storeEntity,
                ProductDelivery = productDeliveryEntity,
                Packaging = packagingEntity
            };
        }

        private ProductType MapToProductType(ProductTypeEntity productTypeEntity)
        {

            return ProductType.Create(
                productTypeEntity.Id,
                productTypeEntity.Name
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

        private Packaging MapToPackaging(PackagingEntity packagingEntity)
        {

            return Packaging.Create(
                packagingEntity.Id,
                packagingEntity.Name
            ).Value;
        }

        private PackagingEntity MapToPackagingEntity(Packaging packaging)
        {

            return new PackagingEntity
            {
                Id = packaging.Id,
                Name = packaging.Name,
            };
        }

        private Store MapToStore(StoreEntity storeEntity)
        {
            return Store.Create(
                storeEntity.Id,
                storeEntity.StoreNumber,
                storeEntity.Name,
                storeEntity.YearOfOpened,
                storeEntity.Phone,
                storeEntity.OwnershipTypeId,
                storeEntity.DistrictId
            ).Value;
        }

        private StoreEntity MapToStoreEntity(Store store)
        {
            return new StoreEntity
            {
                Id = store.Id,
                StoreNumber = store.StoreNumber,
                Name = store.Name,
                YearOfOpened = store.YearOfOpened,
                Phone = store.Phone,
                OwnershipTypeId = store.OwnershipTypeId,
                DistrictId = store.DistrictId
            };
        }

        private ProductDelivery MapToProductDelivery(ProductDeliveryEntity productDeliveryEntity)
        {
            var supplier = productDeliveryEntity.Supplier != null ? MapToSupplier(productDeliveryEntity.Supplier) : null;
            var store = productDeliveryEntity.Store != null ? MapToStore(productDeliveryEntity.Store) : null;

            return ProductDelivery.Create(
                productDeliveryEntity.Id,
                productDeliveryEntity.DeliveryDate,
                productDeliveryEntity.SupplierId,
                productDeliveryEntity.StoreId,
                supplier,
                store
            ).Value;
        }

        private ProductDeliveryEntity MapToProductDeliveryEntity(ProductDelivery productDelivery)
        {
            var supplierEntity = productDelivery.Supplier != null ? MapToSupplierEntity(productDelivery.Supplier) : null;
            var storeEntity = productDelivery.Store != null ? MapToStoreEntity(productDelivery.Store) : null;
            var productForSalesEntities = productDelivery.ProductForSales.Select(pfs => MapToProductForSaleEntity(pfs)).ToList();

            return new ProductDeliveryEntity
            {
                Id = productDelivery.Id,
                DeliveryDate = productDelivery.DeliveryDate,
                SupplierId = productDelivery.SupplierId,
                Supplier = supplierEntity,
                StoreId = productDelivery.StoreId,
                Store = storeEntity,
                ProductForSales = productForSalesEntities
            };
        }

        private Supplier MapToSupplier(SupplierEntity supplierEntity)
        {
            return Supplier.Create(
                supplierEntity.Id,
                supplierEntity.Name,
                supplierEntity.OwnershipTypeId,
                supplierEntity.CityId,
                supplierEntity.Phone
            ).Value;
        }

        private SupplierEntity MapToSupplierEntity(Supplier supplier)
        {
            return new SupplierEntity
            {
                Id = supplier.Id,
                Name = supplier.Name,
                OwnershipTypeId = supplier.OwnershipTypeId,
                CityId = supplier.CityId,
                Phone = supplier.Phone
            };
        }
    }
}
