using CandyKeeper.Domain.Models;
using CandyKeeper.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandyKeeper.DAL.Entities;

namespace CandyKeeper.DAL
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public SupplierRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var supplierEntities = await _context.Suppliers
                    .AsNoTracking()
                    .Include(s => s.OwnershipType)
                    .Include(s => s.City)
                    .Include(s => s.Stores)
                    .ThenInclude(s => s.District)
                    .Include(s => s.ProductDeliveries)
                    .ThenInclude(pd => pd.ProductForSales)
                    .ThenInclude(pfs => pfs.Packaging)
                    .Include(s => s.ProductDeliveries)
                    .ThenInclude(pd => pd.ProductForSales)
                    .ThenInclude(pfs => pfs.Product)
                    .ToListAsync();

                var suppliers = supplierEntities.Select(s => MapToSupplier(s)).ToList();


                return suppliers;
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

        public async Task<Supplier> GetById(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                var supplierEntity = await _context.Suppliers
                    .Include(s => s.OwnershipType)
                    .Include(s => s.City)
                    .Include(s => s.Stores)
                    .ThenInclude(s => s.District)
                    .Include(s => s.ProductDeliveries)
                    .ThenInclude(pd => pd.ProductForSales)
                    .ThenInclude(pfs => pfs.Packaging)
                    .Include(s => s.ProductDeliveries)
                    .ThenInclude(pd => pd.ProductForSales)
                    .ThenInclude(pfs => pfs.Product)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (supplierEntity == null)
                    throw new Exception("supplier null");


                var supplier = MapToSupplier(supplierEntity);

                return supplier;
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

        public async Task Create(Supplier supplier)
        {
            await _semaphore.WaitAsync();

            try
            {
                var supplierEntity = MapToSupplierEntity(supplier);

                await _context.Suppliers.AddAsync(supplierEntity);
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

        public async Task Update(int id, string name, string phone,
                                 int ownershipTypeId, int cityId)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.Suppliers
                    .Where(p => p.Id == id)
                    .ExecuteUpdateAsync(p => p
                        .SetProperty(p => p.Id, id)
                        .SetProperty(p => p.Name, name)
                        .SetProperty(p => p.Phone, phone)
                        .SetProperty(p => p.OwnershipTypeId, ownershipTypeId)
                        .SetProperty(p => p.CityId, cityId));

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
                await _context.Suppliers
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

        private Supplier MapToSupplier(SupplierEntity supplierEntity)
        {
            var ownershipType = supplierEntity.OwnershipType != null ? MapToOwnershipType(supplierEntity.OwnershipType) : null;
            var city = supplierEntity.City != null ? MapToCity(supplierEntity.City) : null;
            var productDeliveries = supplierEntity.ProductDeliveries.Select(pd => MapToProductDelivery(pd)).ToList();
            var stores = supplierEntity.Stores.Select(s => MapToStore(s)).ToList();

            return Supplier.Create(
                supplierEntity.Id,
                supplierEntity.Name,
                supplierEntity.OwnershipTypeId,
                supplierEntity.CityId,
                supplierEntity.Phone,
                ownershipType,
                city,
                productDeliveries,
                stores
            ).Value;
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

        private ProductDelivery MapToProductDelivery(ProductDeliveryEntity productDeliveryEntity)
        {
            var store = productDeliveryEntity.Store != null ? MapToStore(productDeliveryEntity.Store) : null;
            var productForSales = productDeliveryEntity.ProductForSales.Select(pfs => MapToProductForSale(pfs)).ToList();

            return ProductDelivery.Create(
                productDeliveryEntity.Id,
                productDeliveryEntity.DeliveryDate,
                productDeliveryEntity.SupplierId,
                productDeliveryEntity.StoreId,
                null,
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
                StoreId = productDelivery.StoreId,
                Supplier = supplierEntity,
                Store = storeEntity,
                ProductForSales = productForSalesEntities
            };
        }

        private Store MapToStore(StoreEntity storeEntity)
        {
            var ownershipType = storeEntity.OwnershipType != null ? MapToOwnershipType(storeEntity.OwnershipType) : null;
            var district = storeEntity.District != null ? MapToDistrict(storeEntity.District) : null;
            var productForSales = storeEntity.ProductForSales.Select(pfs => MapToProductForSale(pfs)).ToList();
            var productDeliveries = storeEntity.ProductDeliveries.Select(pd => MapToProductDelivery(pd)).ToList();

            return Store.Create(
                storeEntity.Id,
                storeEntity.StoreNumber,
                storeEntity.Name,
                storeEntity.YearOfOpened,
                storeEntity.Phone,
                storeEntity.OwnershipTypeId,
                storeEntity.DistrictId,
                ownershipType,
                district,
                null,
                productForSales,
                productDeliveries
            ).Value;
        }

        private StoreEntity MapToStoreEntity(Store store)
        {
            var ownershipTypeEntity = store.OwnershipType != null ? MapToOwnershipTypeEntity(store.OwnershipType) : null;
            var districtEntity = store.District != null ? MapToDistrictEntity(store.District) : null;
            var suppliersEntities = store.Suppliers.Select(s => MapToSupplierEntity(s)).ToList();
            var productForSalesEntities = store.ProductForSales.Select(pfs => MapToProductForSaleEntity(pfs)).ToList();
            var productDeliveriesEntities = store.ProductDeliveries.Select(pd => MapToProductDeliveryEntity(pd)).ToList();

            return new StoreEntity
            {
                Id = store.Id,
                StoreNumber = store.StoreNumber,
                Name = store.Name,
                YearOfOpened = store.YearOfOpened,
                Phone = store.Phone,
                OwnershipTypeId = store.OwnershipTypeId,
                DistrictId = store.DistrictId,
                OwnershipType = ownershipTypeEntity,
                District = districtEntity,
                Suppliers = suppliersEntities,
                ProductForSales = productForSalesEntities,
                ProductDeliveries = productDeliveriesEntities
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

        private OwnershipType MapToOwnershipType(OwnershipTypeEntity ownershipTypeEntity)
        {
            return OwnershipType.Create(
                ownershipTypeEntity.Id,
                ownershipTypeEntity.Name
            ).Value;
        }

        private OwnershipTypeEntity MapToOwnershipTypeEntity(OwnershipType ownershipType)
        {
            return new OwnershipTypeEntity
            {
                Id = ownershipType.Id,
                Name = ownershipType.Name
            };
        }

        private District MapToDistrict(DistrictEntity districtEntity)
        {
            var city = districtEntity.City != null ? MapToCity(districtEntity.City) : null;

            return District.Create(
                districtEntity.Id,
                districtEntity.Name,
                districtEntity.CityId,
                city
            ).Value;
        }

        private DistrictEntity MapToDistrictEntity(District district)
        {
            var cityEntity = district.City != null ? MapToCityEntity(district.City) : null;

            return new DistrictEntity
            {
                Id = district.Id,
                Name = district.Name,
                CityId = district.CityId,
                City = cityEntity
            };
        }

        private City MapToCity(CityEntity cityEntity)
        {
            return City.Create(
                cityEntity.Id,
                cityEntity.Name
            ).Value;
        }

        private CityEntity MapToCityEntity(City city)
        {
            return new CityEntity
            {
                Id = city.Id,
                Name = city.Name
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

        private Product MapToProduct(ProductEntity productEntity)
        {
            var productType = productEntity.ProductType != null ? MapToProductType(productEntity.ProductType) : null;

            return Product.Create(
                productEntity.Id,
                productEntity.Name,
                productEntity.ProductTypeId,
                productType
            ).Value;
        }

        private ProductEntity MapToProductEntity(Product product)
        {
            var productTypeEntity = product.ProductType != null ? MapToProductTypeEntity(product.ProductType) : null;

            return new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                ProductTypeId = product.ProductTypeId,
                ProductType = productTypeEntity
            };
        }

        private ProductType MapToProductType(ProductTypeEntity productTypeEntity)
        {
            var products = productTypeEntity.Products.Select(p => MapToProduct(p)).ToList();

            return ProductType.Create(productTypeEntity.Id, productTypeEntity.Name, products).Value;
        }

        private ProductTypeEntity MapToProductTypeEntity(ProductType productType)
        {
            var productsEntities = productType.Products.Select(p => MapToProductEntity(p)).ToList();

            return new ProductTypeEntity
            {
                Id = productType.Id,
                Name = productType.Name,
                Products = productsEntities
            };
        }

    }
}
