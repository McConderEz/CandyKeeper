using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class Store
    {
        private static readonly Regex ValidationRegex = new Regex(
                            @"(^\+\d{1,3}\d{10}$|^$)",
                            RegexOptions.Singleline | RegexOptions.Compiled);
        public const int MAX_NAME_SIZE = 100;
        public const int STORE_NUMBER_MIN = 1000000;
        public const int STORE_NUMBER_MAX = 9999999;

        private ICollection<Supplier> _suppliers = [];
        private ICollection<ProductForSale> _productsForSale = [];
        private ICollection<ProductDelivery> _productsDelivery = [];

        private Store(int id,int storeNumber, string name, DateTime yearOfOpened, string phone,int ownershipTypeId,int districtId,
            IEnumerable<Supplier> suppliers, IEnumerable<ProductForSale> productForSales, IEnumerable<ProductDelivery> productDeliveries)
        {
            Id = id;
            StoreNumber = storeNumber;
            Name = name;
            YearOfOpened = yearOfOpened;
            Phone = phone;
            OwnershipTypeId = ownershipTypeId;
            DistrictId = districtId;
            AddSupplier(suppliers.ToList());
            AddProductForSale(productForSales.ToList());
            AddProductDelivery(productDeliveries.ToList());
        }

        public int Id { get; }
        public int StoreNumber { get; }
        public string Name { get; } = string.Empty;
        public DateTime YearOfOpened { get; }
        public int NumberOfEmployees { get; private set; } = 0;
        public string Phone { get; } = string.Empty;
        public int OwnershipTypeId { get; }
        public virtual OwnershipType? OwnershipType { get; }
        public int DistrictId { get; }
        public virtual District? District { get; }
        public IReadOnlyCollection<Supplier> Suppliers => _suppliers.ToList().AsReadOnly();
        public IReadOnlyCollection<ProductForSale> ProductsForSale => _productsForSale.ToList().AsReadOnly();
        public IReadOnlyCollection<ProductDelivery> ProductsDelivery => _productsDelivery.ToList().AsReadOnly();

        public void AddSupplier(List<Supplier> suppliers) => _suppliers.ToList().AddRange(suppliers);
        public void AddProductForSale(List<ProductForSale> productForSales) => _productsForSale.ToList().AddRange(productForSales);
        public void AddProductDelivery(List<ProductDelivery> productDeliveries) => _productsDelivery.ToList().AddRange(productDeliveries);

        public void CountNumberOfEmployees() => NumberOfEmployees++;

        public static Result<Store> Create(int id,int storeNumber, string name, DateTime yearOfOpened, string phone, int ownershipTypeId, int districtId,
                      IEnumerable<Supplier> suppliers = null, IEnumerable<ProductForSale> productForSales = null, IEnumerable<ProductDelivery> productDeliveries = null)
        {
            if (storeNumber < STORE_NUMBER_MIN || storeNumber > STORE_NUMBER_MAX)
                return Result.Failure<Store>("storeNumber is in incorrect ranger");

            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<Store>("name cannot be null or length > 80");

            if(!ValidationRegex.IsMatch(phone))
                return Result.Failure<Store>("phone has incorrect format");

            if(yearOfOpened.Year < 1860 || yearOfOpened > DateTime.Now)
                return Result.Failure<Store>("Incorrect date");

            

            var store = new Store(id,storeNumber, name, yearOfOpened, phone, ownershipTypeId, districtId,
                                  suppliers ?? Enumerable.Empty<Supplier>(),
                                  productForSales ?? Enumerable.Empty<ProductForSale>(),
                                  productDeliveries ?? Enumerable.Empty<ProductDelivery>());

            return Result.Success(store);
        }

    }
}
