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
    public class StoreRepository : IStoreRepository
    {
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public StoreRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<Store>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var storeEntities = await _context.Stores
                    .AsNoTracking()
                    .Include(s => s.OwnershipType)
                    .Include(s => s.District)
                        .ThenInclude(d => d.City)
                    .Include(s => s.Suppliers)
                        .ThenInclude(s => s.OwnershipType)
                    .Include(s => s.Suppliers)
                        .ThenInclude(s => s.City)
                    .Include(s => s.ProductForSales)
                        .ThenInclude(pfs => pfs.Product)
                            .ThenInclude(p => p.ProductType)
                    .Include(s => s.ProductForSales)
                        .ThenInclude(pfs => pfs.Packaging)
                    .Include(s => s.ProductDeliveries)
                        .ThenInclude(pd => pd.Supplier)
                    .ToListAsync();

                var stores = storeEntities.Select(s => MapToStore(s)).ToList();


                return stores;
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

        public async Task<Store> GetById(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                var storeEntity = await _context.Stores
                    .Include(s => s.OwnershipType)
                    .Include(s => s.District)
                        .ThenInclude(d => d.City)
                    .Include(s => s.Suppliers)
                        .ThenInclude(s => s.OwnershipType)
                    .Include(s => s.Suppliers)
                        .ThenInclude(s => s.City)
                    .Include(s => s.ProductForSales)
                        .ThenInclude(pfs => pfs.Product)
                            .ThenInclude(p => p.ProductType)
                    .Include(s => s.ProductForSales)
                        .ThenInclude(pfs => pfs.Packaging)
                    .Include(s => s.ProductForSales)
                        .ThenInclude(pfs => pfs.ProductDelivery)
                            .ThenInclude(pfs => pfs.Supplier)
                    .Include(s => s.ProductDeliveries)
                        .ThenInclude(pd => pd.Supplier)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (storeEntity == null)
                    throw new Exception("store null");


                var store = MapToStore(storeEntity);

                return store;
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

        public async Task Create(Store store)
        {
            await _semaphore.WaitAsync();

            try
            {
                var storeEntity = MapToStoreEntity(store);

                await _context.Stores.AddAsync(storeEntity);
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

        public async Task Update(int id, int storeNumber, string name, DateTime yearOfOpened, int numberOfEmployees, string phone,
                                 int ownershipTypeId, int districtId)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.Stores
                    .Where(p => p.Id == id)
                    .ExecuteUpdateAsync(p => p
                        .SetProperty(p => p.Id, id)
                        .SetProperty(p => p.StoreNumber, storeNumber)
                        .SetProperty(p => p.Name, name)
                        .SetProperty(p => p.YearOfOpened, yearOfOpened)
                        .SetProperty(p => p.NumberOfEmployees, numberOfEmployees)
                        .SetProperty(p => p.Phone, phone)
                        .SetProperty(p => p.OwnershipTypeId, ownershipTypeId)
                        .SetProperty(p => p.DistrictId, districtId));

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
                await _context.Stores
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
        
        public async Task AddSupplier(int id,Supplier model)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (model == null)
                    throw new NullReferenceException("model is null");
                
                var entity = await _context.Suppliers.FindAsync(model.Id);

                if (entity == null)
                    throw new NullReferenceException("entity is null");
                
                var store = await _context.Stores.SingleOrDefaultAsync(pd => pd.Id == id);

                if (store == null)
                    throw new NullReferenceException("store is null");
                

                
                store.Suppliers.Add(entity);
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
        
        public async Task DeleteSupplier(int id,Supplier model)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (model == null)
                    throw new NullReferenceException("model is null");
                
                var entity = await _context.Suppliers.FindAsync(model.Id);

                if (entity == null)
                    throw new NullReferenceException("entity is null");
                
                var store = await _context.Stores.SingleOrDefaultAsync(pd => pd.Id == id);

                if (store == null)
                    throw new NullReferenceException("store is null");
                

                
                store.Suppliers.Remove(entity);
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

        private Store MapToStore(StoreEntity storeEntity)
        {
            var ownershipType = storeEntity.OwnershipType != null ? MapToOwnershipType(storeEntity.OwnershipType) : null;
            var district = storeEntity.District != null ? MapToDistrict(storeEntity.District) : null;
            var suppliers = storeEntity.Suppliers.Select(s => MapToSupplier(s)).ToList();
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
                suppliers,
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

        private Supplier MapToSupplier(SupplierEntity supplierEntity)
        {
            var ownershipType = supplierEntity.OwnershipType != null ? MapToOwnershipType(supplierEntity.OwnershipType) : null;
            var city = supplierEntity.City != null ? MapToCity(supplierEntity.City) : null;

            return Supplier.Create(
                supplierEntity.Id,
                supplierEntity.Name,
                supplierEntity.OwnershipTypeId,
                supplierEntity.CityId,
                supplierEntity.Phone,
                ownershipType,
                city
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

        private ProductForSale MapToProductForSale(ProductForSaleEntity productForSaleEntity)
        {
            var product = productForSaleEntity.Product != null ? MapToProduct(productForSaleEntity.Product) : null;
            var packaging = productForSaleEntity.Packaging != null ? MapToPackaging(productForSaleEntity.Packaging) : null;
            var productDelivery = productForSaleEntity.ProductDelivery != null ? MapToProductDelivery(productForSaleEntity.ProductDelivery) : null;
            
            return ProductForSale.Create(
                productForSaleEntity.Id,
                productForSaleEntity.ProductId,
                productForSaleEntity.StoreId,
                productForSaleEntity.ProductDeliveryId,
                productForSaleEntity.PackagingId,
                productForSaleEntity.Price,
                productForSaleEntity.Volume,
                product,
                null,
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

        private ProductDelivery MapToProductDelivery(ProductDeliveryEntity productDeliveryEntity)
        {         
            var supplier = MapToSupplier(productDeliveryEntity.Supplier);
            
            return ProductDelivery.Create(
                productDeliveryEntity.Id,
                productDeliveryEntity.DeliveryDate,
                productDeliveryEntity.SupplierId,
                productDeliveryEntity.StoreId,
                supplier,
                null
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
            return ProductType.Create(productTypeEntity.Id, productTypeEntity.Name).Value;
        }

        private ProductTypeEntity MapToProductTypeEntity(ProductType productType)
        {

            return new ProductTypeEntity
            {
                Id = productType.Id,
                Name = productType.Name
            };
        }

    }
}
