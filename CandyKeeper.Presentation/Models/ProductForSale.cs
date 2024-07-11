using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.Models
{
    public class ProductForSale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int StoreId { get; set; }
        public virtual Store? Store { get; set; }
        public int ProductDeliveryId { get; set; }
        public virtual ProductDelivery? ProductDelivery { get; set; }
        public int PackagingId { get; set; }
        public virtual Packaging? Packaging { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
    }
}
