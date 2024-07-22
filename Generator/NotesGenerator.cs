using CandyKeeper.DAL;
using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using CandyKeeper.DAL.Entities;

namespace Generator
{
    public class NotesGenerator
    {
        private readonly CandyKeeperDbContext _dbContext = new CandyKeeperDbContext(null);
        private readonly Parser _parser = new Parser();

        private const string PATH = "C:\\Users\\rusta\\source\\repos\\CandyKeeper\\Generator\\Dataset\\";

        public void GenAll()
        {
            //GenCity();
            //GenDistrict();
            //GenOwnershipType();
            //GenProductType();
            //GenPackaging();
            //GenProduct();
            //GenSupplier();
            //GenStore();
            //GenProductDelivery();
            GenProductForSale();
        }



        public void GenCity()
        {
            var list = _parser.Parse(PATH + "City.txt");
            for (var i = 0; i < list.Count; i++)
            {
                _dbContext.Cities.Add(new CityEntity { Name = list[i] });
            }
            _dbContext.SaveChanges();
        }


        public void GenDistrict()
        {
            var list = _parser.ParseCSV(PATH + "District.csv");
            for (var i = 0; i < list.Count; i++)
            {
                var city = _dbContext.Cities.First(c => c.Name.Equals(list[i].Item1));
                for (var j = 0; j < list[i].Item2.Count(); j++)
                {
                    _dbContext.Districts.Add(new DistrictEntity { Name = list[i].Item2[j], CityId = city.Id });
                }
            }
            _dbContext.SaveChanges();
        }

        public void GenOwnershipType()
        {
            var list = _parser.Parse(PATH + "OwnershipType.txt");
            for (var i = 0; i < list.Count; i++)
            {
                _dbContext.OwnershipTypes.Add(new OwnershipTypeEntity { Name = list[i] });
            }
            _dbContext.SaveChanges();
        }


        public void GenProductType()
        {
            var list = _parser.Parse(PATH + "ProductType.txt");
            for (var i = 0; i < list.Count; i++)
            {
                _dbContext.ProductTypes.Add(new ProductTypeEntity { Name = list[i] });
            }
            _dbContext.SaveChanges();
        }

        public void GenPackaging()
        {
            var list = _parser.Parse(PATH + "Packaging.txt");
            for (var i = 0; i < list.Count; i++)
            {
                _dbContext.Packaging.Add(new PackagingEntity { Name = list[i] });
            }
            _dbContext.SaveChanges();
        }


        public void GenProduct()
        {
            var list = _parser.ParseCSV(PATH + "Product.csv");

            for (var i = 0; i < list.Count; i++)
            {
                var productType = _dbContext.ProductTypes.First(c => c.Name.Equals(list[i].Item2.First()));
                _dbContext.Products.Add(new ProductEntity { Name = list[i].Item1, ProductTypeId = productType.Id });
            }
            _dbContext.SaveChanges();
        }

        public static string GeneratePhoneNumber()
        {
            Random random = new Random();

            int countryCodeLength = random.Next(1, 4);
            string countryCode = GenerateRandomDigits(countryCodeLength, random);

            string mainNumber = GenerateRandomDigits(10, random);

            string phoneNumber = $"+{countryCode}{mainNumber}";

            return phoneNumber;
        }

        private static string GenerateRandomDigits(int length, Random random)
        {
            char[] digits = new char[length];
            for (int i = 0; i < length; i++)
            {
                digits[i] = (char)('0' + random.Next(0, 10));
            }

            return new string(digits);
        }

        public static DateTime GenerateRandomDate(DateTime startDate, DateTime endDate)
        {
            Random random = new Random();
            int range = (endDate - startDate).Days;
            return startDate.AddDays(random.Next(range));
        }

        public void GenSupplier()
        {
            var list = _parser.Parse(PATH + "SuppliersName.txt");
            var cityIds = _dbContext.Cities.Select(c => c.Id).ToList();
            var ownershipTypeIds = _dbContext.OwnershipTypes.Select(o => o.Id).ToList();
            DateTime startDate = new DateTime(1930, 1, 1);
            DateTime endDate = DateTime.Now;

            for (var i = 0; i < list.Count; i++)
            {
                for(var j = 0; j < 5; j++)
                {
                    _dbContext.Suppliers.Add(new SupplierEntity
                    {
                        Name = list[i],
                        CityId = cityIds[new Random().Next(0, cityIds.Count - 1)],
                        OwnershipTypeId = ownershipTypeIds[new Random().Next(0, ownershipTypeIds.Count - 1)],
                        Phone = GeneratePhoneNumber()
                    });
                }
            }
            _dbContext.SaveChanges();
        }

        public void GenStore()
        {
            var list = _parser.Parse(PATH + "SuppliersName.txt");
            var districtIds = _dbContext.Districts.Select(d => d.Id).ToList();
            var ownershipTypeIds = _dbContext.OwnershipTypes.Select(o => o.Id).ToList();
            DateTime startDate = new DateTime(1930, 1, 1);
            DateTime endDate = DateTime.Now;

            for (var i = 0; i < list.Count; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    _dbContext.Stores.Add(new StoreEntity
                    {
                        Name = list[i],
                        StoreNumber = new Random().Next(100000,1000000),
                        DistrictId = districtIds[new Random().Next(0, districtIds.Count - 1)],
                        OwnershipTypeId = ownershipTypeIds[new Random().Next(0, ownershipTypeIds.Count - 1)],
                        Phone = GeneratePhoneNumber(),
                        YearOfOpened = GenerateRandomDate(startDate, endDate)
                    });
                }
            }
            _dbContext.SaveChanges();
        }

        public void GenProductDelivery()
        {
            var supplierIds = _dbContext.Suppliers.Select(s => s.Id).ToList();
            var storeIds = _dbContext.Stores.Select(s => s.Id).ToList();
            DateTime startDate = new DateTime(2015, 1, 1);
            DateTime endDate = DateTime.Now;

            for (var i = 0; i < storeIds.Count; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    _dbContext.ProductDeliveries.Add(new ProductDeliveryEntity
                    {
                        DeliveryDate = GenerateRandomDate(startDate, endDate),
                        SupplierId = supplierIds[new Random().Next(0, supplierIds.Count - 1)],
                        StoreId = storeIds[i]
                    });
                }
            }
            _dbContext.SaveChanges();
        }


        public void GenProductForSale()
        {
            var deliveries = _dbContext.ProductDeliveries.ToList();
            var storeIds = _dbContext.Stores.Select(s => s.Id).ToList();
            var productIds = _dbContext.Products.Select(p => p.Id).ToList();
            var packagingIds = _dbContext.Packaging.Select(p => p.Id).ToList();
            DateTime startDate = new DateTime(2015, 1, 1);
            DateTime endDate = DateTime.Now;

            for (var i = 0; i < deliveries.Count; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    _dbContext.ProductForSales.Add(new ProductForSaleEntity
                    {
                        ProductId = productIds[new Random().Next(0, productIds.Count - 1)],
                        StoreId = deliveries[i].StoreId,
                        ProductDeliveryId = deliveries[i].Id,
                        PackagingId = packagingIds[new Random().Next(0, packagingIds.Count - 1)],
                        Price = new Random().Next(10, 1000000),
                        Volume = new Random().Next(1, 100000)
                    });
                }
            }
            _dbContext.SaveChanges();
        }
    }
    
}
