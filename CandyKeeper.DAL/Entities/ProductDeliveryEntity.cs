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
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int SupplierId { get; set; }
        public virtual SupplierEntity? Supplier { get; set; }
        public int StoreId { get; set; }
        public virtual StoreEntity? Store { get; set; }
        public ICollection<ProductForSaleEntity> ProductForSales { get; set; } = [];
    }
}
