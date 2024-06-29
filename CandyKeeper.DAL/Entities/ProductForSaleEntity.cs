using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Entities
{
    public class ProductForSaleEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual ProductEntity? Product { get; set; }
        public int StoreId { get; set; }
        public virtual StoreEntity? Store { get; set; }
        public int ProductDeliveryId { get; set; }
        public virtual ProductDeliveryEntity? ProductDelivery { get; set; }
        public int PackagingId { get; set; }
        public virtual PackagingEntity? Packaging { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
    }
}
