using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class ProductType
    {
        public const int MAX_NAME_SIZE = 100;

        private List<Product> _products = [];

        private ProductType(int id,string name, IEnumerable<Product> products)
        {
            Id = id;
            Name = name;
            AddProduct(products.ToList());
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public void AddProduct(List<Product> products) => _products.AddRange(products);

        public static Result<ProductType> Create(int id, string name, IEnumerable<Product> products = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<ProductType>("name cannot be null or length > 100");

            var productType = new ProductType(id, name, products ?? Enumerable.Empty<Product>());

            return Result.Success(productType);
        }
    }
}
