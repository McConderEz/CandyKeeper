using CSharpFunctionalExtensions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CandyKeeper.Domain.Models
{
    public class City
    {
        public const int MAX_NAME_LENGTH = 80;

        private List<District> _districts = [];
        private List<Supplier> _suppliers = [];

        private City(int id, string name, IEnumerable<District> districts, IEnumerable<Supplier> suppliers)
        {
            Id = id;
            Name = name;
            AddDistrict(districts.ToList());
            AddSupplier(suppliers.ToList());
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public IReadOnlyCollection<District> Districts => _districts.ToList().AsReadOnly();
        public IReadOnlyCollection<Supplier> Suppliers => _suppliers.ToList().AsReadOnly();

        public void AddDistrict(List<District> districts) => _districts.AddRange(districts);
        public void AddSupplier(List<Supplier> suppliers) => _suppliers.AddRange(suppliers);

        public static Result<City> Create(int id,string name, IEnumerable<District> districts = null, IEnumerable<Supplier> suppliers = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 80)
                return Result.Failure<City>("name cannot be null or length > 80");

            var city = new City(id, name,
                                districts ?? Enumerable.Empty<District>(),
                                suppliers ?? Enumerable.Empty<Supplier>());
            
            return Result.Success(city);
        }

    }
}
