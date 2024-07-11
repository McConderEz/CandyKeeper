using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.Models
{
    public class ProductDelivery
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int SupplierId { get; set; }
        public virtual Domain.Models.Supplier? Supplier { get; set; }
        public int StoreId { get; set; }
        public virtual Store? Store { get; set; }
        public ICollection<ProductForSale> ProductForSales { get; set; } = [];
    }
}
