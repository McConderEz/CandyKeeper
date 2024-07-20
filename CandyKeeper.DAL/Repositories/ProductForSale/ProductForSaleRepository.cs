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
    public class ProductForSaleRepository : IProductForSaleRepository
    {
        private readonly CandyKeeperDbContext _context;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public ProductForSaleRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductForSale>> Get()
        {
            await _semaphore.WaitAsync();

            try
            {
                var productForSalesEntities = await _context.ProductForSales
                    .AsNoTracking()
                    .Include(pfs => pfs.Product)
                    .ThenInclude(p => p.ProductType)
                    .Include(pfs => pfs.Packaging)
                    .Include(pfs => pfs.Store)
                    .ThenInclude(s => s.District)
                    .ThenInclude(d => d.City)
                    .ToListAsync();

                var productForSales = productForSalesEntities.Select(pfs => MapToProductForSale(pfs)).ToList();


                return productForSales;
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

        public async Task<ProductForSale> GetById(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                IQueryable<ProductForSaleEntity> productForSaleEntity = _context.ProductForSales.Where(pfs => pfs.Id == id).AsNoTracking();

                if (productForSaleEntity == null)
                    throw new Exception("productForSale null");


                var productForSale = MapToProductForSale((await productForSaleEntity
                    .Include(pfs => pfs.Product)
                    .ThenInclude(p => p.ProductType)
                    .Include(pfs => pfs.Packaging)
                    .Include(pfs => pfs.Store)
                    .ThenInclude(s => s.District)
                    .ThenInclude(d => d.City)
                    .Include(pfs => pfs.ProductDelivery)
                    .ThenInclude(pd => pd.Supplier)
                    .SingleOrDefaultAsync())!);

                return productForSale;
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

        public async Task Create(ProductForSale productForSale)
        {
            await _semaphore.WaitAsync();

            try
            {
                var productForSaleEntity = MapToProductForSaleEntity(productForSale);

                await _context.ProductForSales.AddAsync(productForSaleEntity);
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

        public async Task Update(int id, decimal price, int volume, int productId, int storeId, int productDeliveryId, int packagingId)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.ProductForSales
                    .Where(p => p.Id == id)
                    .ExecuteUpdateAsync(p => p
                        .SetProperty(p => p.Price, price)
                        .SetProperty(p => p.Volume, volume)
                        .SetProperty(p => p.ProductId, productId)
                        .SetProperty(p => p.StoreId, storeId)
                        .SetProperty(p => p.ProductDeliveryId, productDeliveryId)
                        .SetProperty(p => p.PackagingId, packagingId));

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
                await _context.ProductForSales
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

            return Product.Create(
                productEntity.Id,
                productEntity.Name,
                productEntity.ProductTypeId,
                productType
            ).Value;
        }

        private ProductEntity MapToProductEntity(Product product)
        {

            return new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                ProductTypeId = product.ProductTypeId,
            };
        }

        private ProductType MapToProductType(ProductTypeEntity productTypeEntity)
        {
            return ProductType.Create(productTypeEntity.Id, productTypeEntity.Name).Value;
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

        private ProductForSale MapToProductForSale(ProductForSaleEntity productForSaleEntity)
        {
            var product = productForSaleEntity.Product != null ? MapToProduct(productForSaleEntity.Product) : null;
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
                product,
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

        private Store MapToStore(StoreEntity storeEntity)
        {
            var district = storeEntity.District != null ? MapToDistrict(storeEntity.District) : null;
            var ownershipType = storeEntity.OwnershipType != null ? MapToOwnershipType(storeEntity.OwnershipType) : null;

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
                storeEntity.Suppliers.Select(s => MapToSupplier(s)).ToList()
            ).Value;
        }

        private Supplier MapToSupplier(SupplierEntity s)
        {
            return Supplier.Create(s.Id, s.Name, s.OwnershipTypeId, s.CityId, s.Phone).Value;
        }

        private OwnershipType MapToOwnershipType(OwnershipTypeEntity ownershipType)
        {
            return OwnershipType.Create(ownershipType.Id, ownershipType.Name).Value;
        }

        private StoreEntity MapToStoreEntity(Store store)
        {
            var districtEntity = store.District != null ? MapToDistrictEntity(store.District) : null;

            return new StoreEntity
            {
                Id = store.Id,
                StoreNumber = store.StoreNumber,
                Name = store.Name,
                YearOfOpened = store.YearOfOpened,
                Phone = store.Phone,
                OwnershipTypeId = store.OwnershipTypeId,
                DistrictId = store.DistrictId,
                District = districtEntity,
                Suppliers = store.Suppliers.Select(s => MapToSupplierEntity(s)).ToList()
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
                StoreId = productDelivery.StoreId,
                Supplier = supplierEntity,
                Store = storeEntity,
                ProductForSales = productForSalesEntities
            };
        }

        private SupplierEntity MapToSupplierEntity(Supplier supplier)
        {
            return new SupplierEntity
            {
                Id = supplier.Id,
                Name = supplier.Name,
                CityId = supplier.CityId,
                OwnershipTypeId = supplier.OwnershipTypeId,
                Phone = supplier.Phone,
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
