using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using CandyKeeper.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL
{
    public class ProductDeliveryRepository : IProductDeliveryRepository
    {
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public ProductDeliveryRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDelivery>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var productDeliveriesEntities = await _context.ProductDeliveries
                    .AsNoTracking()
                    .Include(pd => pd.Supplier)
                    .Include(pd => pd.ProductForSales)
                    .ThenInclude(pfs => pfs.Product)
                    .ThenInclude(p => p.ProductType)
                    .Include(pd => pd.ProductForSales)
                    .ThenInclude(pfs => pfs.Packaging)
                    .Include(pd => pd.Store)
                    .ToListAsync();

                var productDeliveries = productDeliveriesEntities.Select(pd => MapToProductDelivery(pd)).ToList();


                return productDeliveries;
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

        public async Task<ProductDelivery> GetById(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                var productDeliveryEntity = await _context.ProductDeliveries
                    .Include(pd => pd.Supplier)
                    .Include(pd => pd.ProductForSales)
                    .ThenInclude(pfs => pfs.Product)
                    .ThenInclude(p => p.ProductType)
                    .Include(pd => pd.ProductForSales)
                    .ThenInclude(pfs => pfs.Packaging)
                    .Include(pd => pd.Store)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (productDeliveryEntity == null)
                    throw new Exception("productDelivery null");


                var productDelivery = MapToProductDelivery(productDeliveryEntity);

                return productDelivery;
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

        public async Task Create(ProductDelivery productDelivery)
        {
            await _semaphore.WaitAsync();

            try
            {
                var productDeliveryEntity = MapToProductDeliveryEntity(productDelivery);

                await _context.ProductDeliveries.AddAsync(productDeliveryEntity);
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

        public async Task Update(int id, DateTime deliveryDate, int supplierId, int storeId)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.ProductDeliveries
                    .Where(p => p.Id == id)
                    .ExecuteUpdateAsync(p => p
                        .SetProperty(p => p.DeliveryDate, deliveryDate)
                        .SetProperty(p => p.SupplierId, supplierId)
                        .SetProperty(p => p.StoreId, storeId));

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
                await _context.ProductDeliveries
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

        public async Task AddProductForSale(int id,ProductForSale model)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (model == null)
                    throw new Exception("entity is null!");

                var productDelivery = await _context.ProductDeliveries.SingleOrDefaultAsync(pd => pd.Id == id);

                if (productDelivery == null)
                    throw new Exception("product delivery is not exist!");

                var entity = MapToProductForSaleEntity(model);
                
                productDelivery.ProductForSales.Add(entity);
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
        
        private ProductDelivery MapToProductDelivery(ProductDeliveryEntity productDeliveryEntity)
        {
            var supplier = productDeliveryEntity.Supplier != null ? MapToSupplier(productDeliveryEntity.Supplier) : null;
            var store = productDeliveryEntity.Store != null ? MapToStore(productDeliveryEntity.Store) : null;
            var productForSales = productDeliveryEntity.ProductForSales.Select(pfs => MapToProductForSale(pfs)).ToList();

            return ProductDelivery.Create(
                productDeliveryEntity.Id,
                productDeliveryEntity.DeliveryDate,
                productDeliveryEntity.SupplierId,
                productDeliveryEntity.StoreId,
                supplier,
                store,
                productForSales
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

        private ProductForSale MapToProductForSale(ProductForSaleEntity productForSaleEntity)
        {
            var product = productForSaleEntity.Product != null ? MapToProduct(productForSaleEntity.Product) : null;
            var store = productForSaleEntity.Store != null ? MapToStore(productForSaleEntity.Store) : null;         
            var packaging = productForSaleEntity.Packaging != null ? MapToPackaging(productForSaleEntity.Packaging) : null;

            return ProductForSale.Create(
                productForSaleEntity.Id,
                productForSaleEntity.ProductId,
                productForSaleEntity.StoreId,
                productForSaleEntity.ProductDeliveryId,
                productForSaleEntity.PackagingId,
                productForSaleEntity.Price,
                productForSaleEntity.Volume,
                product,
                store,
                null,
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

        private Supplier MapToSupplier(SupplierEntity supplierEntity)
        {
            var ownershipType = supplierEntity.OwnershipType != null ? MapToOwnershipType(supplierEntity.OwnershipType) : null;
            var city = supplierEntity.City != null ? MapToCity(supplierEntity.City) : null;
            var stores = supplierEntity.Stores.Select(s => MapToStore(s)).ToList();

            return Supplier.Create(
                supplierEntity.Id,
                supplierEntity.Name,
                supplierEntity.OwnershipTypeId,
                supplierEntity.CityId,
                supplierEntity.Phone,
                ownershipType,
                city,
                null,
                stores
            ).Value;
        }

        private City MapToCity(CityEntity city)
        {
            return City.Create(city.Id, city.Name).Value;
        }

        private OwnershipType MapToOwnershipType(OwnershipTypeEntity ownershipType)
        {
            var stores = ownershipType.Stores.Select(s => MapToStore(s)).ToList();
            var suppliers = ownershipType.Suppliers.Select(s => MapToSupplier(s)).ToList();

            return OwnershipType.Create(ownershipType.Id, ownershipType.Name, stores, suppliers).Value;
        }

        private SupplierEntity MapToSupplierEntity(Supplier supplier)
        {
            var ownershipTypeEntity = supplier.OwnershipType != null ? MapToOwnershipTypeEntity(supplier.OwnershipType) : null;
            var cityEntity = supplier.City != null ? MapToCityEntity(supplier.City) : null;
            var productDeliveriesEntities = supplier.ProductDeliveries.Select(pd => MapToProductDeliveryEntity(pd)).ToList();
            var storesEntities = supplier.Stores.Select(s => MapToStoreEntity(s)).ToList();

            return new SupplierEntity
            {
                Id = supplier.Id,
                Name = supplier.Name,
                OwnershipTypeId = supplier.OwnershipTypeId,
                CityId = supplier.CityId,
                Phone = supplier.Phone,
                OwnershipType = ownershipTypeEntity,
                City = cityEntity,
                ProductDeliveries = productDeliveriesEntities,
                Stores = storesEntities
            };
        }

        private CityEntity MapToCityEntity(City city)
        {
            return new CityEntity
            {
                Id = city.Id,
                Name = city.Name
            };
        }

        private OwnershipTypeEntity MapToOwnershipTypeEntity(OwnershipType ownershipType)
        {
            return new OwnershipTypeEntity
            {
                Id = ownershipType.Id,
                Name = ownershipType.Name
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

        private Product MapToProduct(ProductEntity productEntity)
        {
            return Product.Create(
                productEntity.Id,
                productEntity.Name,
                productEntity.ProductTypeId,
                MapToProductType(productEntity.ProductType)
            ).Value;
        }

        private ProductType MapToProductType(ProductTypeEntity productType)
        {
            return ProductType.Create(productType.Id, productType.Name).Value;
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
                Name = packaging.Name
            };
        }
    }
}
