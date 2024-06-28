using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class OwnershipType
    {
        public const int MAX_NAME_SIZE = 100;

        private ICollection<Store> _stores = [];
        private ICollection<Supplier> _suppliers = [];

        private OwnershipType(int id, string name, IEnumerable<Store> stores, IEnumerable<Supplier> suppliers)
        {
            Id = id;
            Name = name;
            AddStore(stores.ToList());
            AddSupplier(suppliers.ToList());
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public IReadOnlyCollection<Store> Stores => _stores.ToList().AsReadOnly();
        public IReadOnlyCollection<Supplier> Suppliers => _suppliers.ToList().AsReadOnly();

        public void AddStore(List<Store> stores) => _stores.ToList().AddRange(stores);
        public void AddSupplier(List<Supplier> suppliers) => _suppliers.ToList().AddRange(suppliers);

        public static Result<OwnershipType> Create(int id,string name, IEnumerable<Store> stores = null, IEnumerable<Supplier> suppliers = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<OwnershipType>("name cannot be null or length > 100");

            var ownershipType = new OwnershipType(id,name,stores ?? Enumerable.Empty<Store>(),
                                                          suppliers ?? Enumerable.Empty<Supplier>());

            return Result.Success(ownershipType);
        }


    }
}
