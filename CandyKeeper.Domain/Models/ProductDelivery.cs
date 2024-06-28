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
        private ICollection<ProductForSale> _productForSales = [];

        private ProductDelivery(int id,DateTime deliveryDate, int supplierId, int storeId, IEnumerable<ProductForSale> productForSales)
        {
            Id = id;
            DeliveryDate = deliveryDate;
            SupplierId = supplierId;
            StoreId = storeId;
            AddProductForSale(productForSales.ToList());
        }

        public int Id { get; }
        public DateTime DeliveryDate { get; }
        public int SupplierId { get; }
        public virtual Supplier? Supplier { get; }
        public int StoreId { get; }
        public virtual Store? Store { get; }
        public IReadOnlyCollection<ProductForSale> ProductForSales => _productForSales.ToList().AsReadOnly();

        public void AddProductForSale(List<ProductForSale> productForSales) => _productForSales.ToList().AddRange(productForSales);

        public static Result<ProductDelivery> Create(int id,DateTime deliveryDate, int supplierId, int storeId, IEnumerable<ProductForSale> productForSales = null)
        {
            if (deliveryDate > DateTime.Now)
                return Result.Failure<ProductDelivery>("DeliveryDate is incorrect");

            var productDelivery = new ProductDelivery(id,deliveryDate, supplierId, storeId, productForSales ?? Enumerable.Empty<ProductForSale>());

            return Result.Success(productDelivery);
        }
    }
}
