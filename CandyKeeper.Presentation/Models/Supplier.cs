using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int OwnershipTypeId { get; set; }
        public virtual OwnershipType? OwnershipType { get; set; }
        public int CityId { get; set; }
        public virtual City? City { get; set; }
        public ICollection<ProductDelivery> ProductDeliveries { get; set; } = [];
        public ICollection<Store> Stores { get; set; } = [];
    }
}
