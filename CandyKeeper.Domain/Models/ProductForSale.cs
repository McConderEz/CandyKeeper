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

        private ProductForSale(int productId, int storeId, int productDeliveriesId, decimal price, int supplierId, int volume)
        {
            ProductId = productId;
            StoreId = storeId;
            ProductDeliveriesId = productDeliveriesId;
            Price = price;
            SupplierId = supplierId;
            Volume = volume;
        }

        public int Id { get; }
        public int ProductId { get; }
        public virtual Product? Product { get; }
        public int StoreId { get; }
        public int ProductDeliveriesId { get; }
        public virtual ProductDelivery ProductDeliveries { get; }
        public int SupplierId { get; }
        public virtual Supplier Supplier { get; }
        public decimal Price { get; }
        public int Volume { get; }

        public static Result<ProductForSale> Create(int productId, int storeId, int productDeliveriesId, decimal price, int supplierId, int volume)
        {
            if (price < MIN_PRICE || price > MAX_PRICE)
                return Result.Failure<ProductForSale>("price is in incorrect range");

            if (volume < MIN_VOLUME || volume > MAX_VOLUME)
                return Result.Failure<ProductForSale>("volume is in incorrect range");

            var productForSale = new ProductForSale(productId, storeId, productDeliveriesId, price, supplierId, volume);

            return Result.Success(productForSale);
        }
    }
}
