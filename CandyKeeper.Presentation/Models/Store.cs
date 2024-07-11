using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.Models
{
    public class Store
    {
        public int Id { get; set; }
        public int StoreNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime YearOfOpened { get; set; }
        public int NumberOfEmployees { get; set; } = 0;
        public string Phone { get; set; } = string.Empty;
        public int OwnershipTypeId { get; set; }
        public virtual OwnershipType? OwnershipType { get; set; }
        public int DistrictId { get; set; }
        public virtual District? District { get; set; }

        public ICollection<Domain.Models.Supplier> Suppliers { get; set; } = [];
        public ICollection<ProductForSale> ProductForSales { get; set; } = [];
        public ICollection<ProductDelivery> ProductDeliveries { get; set; } = [];
    }
}
