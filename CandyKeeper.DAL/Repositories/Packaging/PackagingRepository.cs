﻿using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using CandyKeeper.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CandyKeeper.DAL
{
    public class PackagingRepository : IPackagingRepository
    {
        private readonly CandyKeeperDbContext _context;       

        public PackagingRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<Packaging>> Get()
        {
            var packagingEntity = await _context.Packaging
                .AsNoTracking()
                .Include(p => p.ProductForSales)
                .ToListAsync();

            var packaging = packagingEntity.Select(p => MapToPackaging(p)).ToList();

            return packaging;
        }

        public async Task<Packaging> GetById(int id)
        {
            var packagingEntity = await _context.Packaging
                                           .Include(p => p.ProductForSales)
                                           .FirstOrDefaultAsync(c => c.Id == id);

            if (packagingEntity == null)
                throw new Exception("Packaging entity is null");

            var packaging = MapToPackaging(packagingEntity);

            return packaging;
        }

        public async Task Create(Packaging packaging)
        {
            var packagingEntity = MapToPackagingEntity(packaging);

            await _context.Packaging.AddAsync(packagingEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, string name)
        {
            await _context.Packaging
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p
                                   .SetProperty(p => p.Name, name));

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Packaging
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
        }

        private Packaging MapToPackaging(PackagingEntity packagingEntity)
        {
            var productForSales = packagingEntity.ProductForSales.Select(pfs => MapToProductForSale(pfs)).ToList();

            return Packaging.Create(
                packagingEntity.Id,
                packagingEntity.Name,
                productForSales
            ).Value;
        }

        private PackagingEntity MapToPackagingEntity(Packaging packaging)
        {
            var productForSales = packaging.ProductForSales.Select(pfs => MapToProductForSaleEntity(pfs)).ToList();

            return new PackagingEntity
            {
                Id = packaging.Id,
                Name = packaging.Name,
                ProductForSales = productForSales
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
            return ProductDelivery.Create(
                productDeliveryEntity.Id,
                productDeliveryEntity.DeliveryDate,
                productDeliveryEntity.SupplierId,
                productDeliveryEntity.StoreId
            ).Value;
        }

        private ProductDeliveryEntity MapToProductDeliveryEntity(ProductDelivery productDelivery)
        {            
            var productForSalesEntities = productDelivery.ProductForSales?.Select(pfs => MapToProductForSaleEntity(pfs)).ToList() ?? new List<ProductForSaleEntity>();

            return new ProductDeliveryEntity
            {
                Id = productDelivery.Id,
                DeliveryDate = productDelivery.DeliveryDate,
                SupplierId = productDelivery.SupplierId,
                StoreId = productDelivery.StoreId,
                ProductForSales = productForSalesEntities
            };
        }

    }
}
