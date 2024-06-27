using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Entities
{
    public class SupplierEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int OwnershipTypeId { get; set; }
        public virtual OwnershipTypeEntity? OwnershipType { get; set; }
        public int CityId { get; set; }
        public virtual CityEntity? City { get; set; }
        public ICollection<ProductDeliveryEntity> ProductDeliveries { get; set; } = [];
        public ICollection<StoreEntity> Stores { get; set; } = [];
    }
}
