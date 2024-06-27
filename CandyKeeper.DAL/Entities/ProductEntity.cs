using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductTypeId { get; set; }
        public virtual ProductTypeEntity? ProductType { get; set; }
        public int PackagingId { get; set; }
        public virtual PackagingEntity? Packaging { get; set; }
        public ICollection<ProductForSaleEntity> ProductForSales { get; set; } = [];
    }
}
