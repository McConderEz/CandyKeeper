using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<District> Districts { get; set; } = [];
        public ICollection<Domain.Models.Supplier> Suppliers { get; set; } = [];
    }
}
