using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class Supplier
    {
        private ICollection<ProductDelivery> _productDeliveries = [];
        private ICollection<Store> _stores = [];

        private static readonly Regex ValidationRegex = new Regex(
                    @"(^\+\d{1,3}\d{10}$|^$)",
                    RegexOptions.Singleline | RegexOptions.Compiled);
        public const int MAX_NAME_SIZE = 100;

        private Supplier(int id,string name, int ownershipTypeId, int cityId, string phone,
            IEnumerable<ProductDelivery> productDeliveries, IEnumerable<Store> stores)
        {
            Id = id;
            Name = name;
            OwnershipTypeId = ownershipTypeId;
            CityId = cityId;
            Phone = phone;
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public string Phone { get; } = string.Empty;
        public int OwnershipTypeId { get; }
        public virtual OwnershipType? OwnershipType { get; }
        public int CityId { get; }
        public virtual City? City { get; }
        public IReadOnlyCollection<ProductDelivery> ProductDeliveries => _productDeliveries.ToList().AsReadOnly();
        public IReadOnlyCollection<Store> Stores => _stores.ToList().AsReadOnly();

        public void AddProductForSale(List<ProductDelivery> productDeliveries) => _productDeliveries.ToList().AddRange(productDeliveries);
        public void AddStore(List<Store> stores) => _stores.ToList().AddRange(stores);

        public static Result<Supplier> Create(int id,string name, int ownershipTypeId, int cityId, string phone,
                    IEnumerable<ProductDelivery> productDeliveries = null,IEnumerable<Store> stores = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<Supplier>("name cannot be null or length > 100");

            if (!ValidationRegex.IsMatch(phone))
                return Result.Failure<Supplier>("phone has incorrect format");

            var supplier = new Supplier(id,name, ownershipTypeId, cityId, phone,
                                        productDeliveries ?? Enumerable.Empty<ProductDelivery>(),
                                        stores ?? Enumerable.Empty<Store>());

            return Result.Success(supplier);
        }

    }
}
