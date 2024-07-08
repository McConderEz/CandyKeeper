using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace CandyKeeper.Domain.Models
{
    public class ProductForSale 
    {
        public const decimal MAX_PRICE = 1000000;
        public const decimal MIN_PRICE = 0.1m;
        public const int MAX_VOLUME = 1000000;
        public const int MIN_VOLUME = 1;

        private ProductForSale(int id,int productId, int storeId, int productDeliveryId,int packagingId ,decimal price, int volume,
                        Product product, Store store, ProductDelivery productDelivery, Packaging packaging)
        {
            Id = id;
            ProductId = productId;
            StoreId = storeId;
            ProductDeliveryId = productDeliveryId;
            PackagingId = packagingId;
            Price = price;
            Volume = volume;
            Product = product;
            Store = store;
            ProductDelivery = productDelivery;
            Packaging = packaging;
        }

        public int Id { get; }
        public int ProductId { get; }
        public virtual Product? Product { get; }
        public int StoreId { get; }
        public virtual Store? Store { get; }
        public int ProductDeliveryId { get; }
        public virtual ProductDelivery? ProductDelivery { get; }
        public int PackagingId { get; }
        public virtual Packaging? Packaging { get; }
        public decimal Price { get; }
        public int Volume { get; }

        public static Result<ProductForSale> Create(int id,int productId, int storeId, int productDeliveryId,int packagingId ,decimal price, int volume,
                                        Product product = null, Store store = null, ProductDelivery productDelivery = null, Packaging packaging = null)
        {
            if (price < MIN_PRICE || price > MAX_PRICE)
                return Result.Failure<ProductForSale>("price is in incorrect range");

            if (volume < MIN_VOLUME || volume > MAX_VOLUME)
                return Result.Failure<ProductForSale>("volume is in incorrect range");

            var productForSale = new ProductForSale(id,productId, storeId, productDeliveryId,packagingId,price, volume,
                                                product, store, productDelivery, packaging);

            return Result.Success(productForSale);
        }
    }
}
