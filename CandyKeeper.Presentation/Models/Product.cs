using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductTypeId { get; set; }
        public virtual ProductType? ProductType { get; set; }
        public ICollection<ProductForSale> ProductForSales { get; set; } = [];
    }
}
