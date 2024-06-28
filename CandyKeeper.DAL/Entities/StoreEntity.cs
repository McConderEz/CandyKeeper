using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Entities
{
    public class StoreEntity
    {
        public int Id { get; set; }
        public int StoreNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime YearOfOpened { get; set; }
        public int NumberOfEmployees { get; set; } = 0;
        public string Phone { get; set; } = string.Empty;
        public int OwnershipTypeId { get; set; }
        public virtual OwnershipTypeEntity? OwnershipType { get; set; }
        public int DistrictId { get; set; }
        public virtual DistrictEntity? District { get; set; }

        public ICollection<SupplierEntity> Suppliers { get; set; } = [];
        public ICollection<ProductForSaleEntity> ProductForSales { get; set; } = [];
        public ICollection<ProductDeliveryEntity> ProductDeliveries { get; set; } = [];
    }
}
