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

        private OwnershipType(string name)
        {
            Name = name;
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public IReadOnlyCollection<Store> Stores => _stores.ToList().AsReadOnly();

        public void AddStore(Store store) => _stores.Add(store);

        public static Result<OwnershipType> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<OwnershipType>("name cannot be null or length > 100");

            var ownershipType = new OwnershipType(name);

            return Result.Success(ownershipType);
        }


    }
}
