using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Entities
{
    public class ProductDeliveryEntity
    {
        public int Id { get; }
        public DateTime DeliveryDate { get; }
        public int SupplierId { get; }
        public virtual SupplierEntity? Supplier { get; }
        public int StoreId { get; }
        public virtual StoreEntity? Store { get; }
        public ICollection<ProductForSaleEntity> ProductForSales { get; set; } = [];
    }
}
