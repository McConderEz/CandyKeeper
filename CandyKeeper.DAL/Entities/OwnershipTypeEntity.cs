using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Entities
{
    public class OwnershipTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<StoreEntity> Stores { get; set; } = [];
        public ICollection<SupplierEntity> Suppliers { get; set; } = [];
    }
}
