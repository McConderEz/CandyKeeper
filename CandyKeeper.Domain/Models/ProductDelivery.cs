using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class ProductDelivery
    {
        private ICollection<ProductForSale> _productsForSale = [];

        private ProductDelivery(DateTime deliveryDate, int supplierId, int storeId)
        {
            DeliveryDate = deliveryDate;
            SupplierId = supplierId;
            StoreId = storeId;
        }

        public int Id { get; }
        public DateTime DeliveryDate { get; }
        public int SupplierId { get; }
        public virtual Supplier? Supplier { get; }
        public int StoreId { get; }
        public virtual Store? Store { get; }
        public IReadOnlyCollection<ProductForSale> ProductsForSales => _productsForSale.ToList().AsReadOnly();

        public static Result<ProductDelivery> Create(DateTime deliveryDate, int supplierId, int storeId)
        {
            if (deliveryDate > DateTime.Now)
                return Result.Failure<ProductDelivery>("DeliveryDate is incorrect");

            var productDelivery = new ProductDelivery(deliveryDate, supplierId, storeId);

            return Result.Success(productDelivery);
        }
    }
}
