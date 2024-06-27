using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class Packaging
    {
        public const int MAX_NAME_SIZE = 100;

        private ICollection<Product> _products = [];

        private Packaging(int id,string name, IEnumerable<Product> products)
        {
            Id = id;
            Name = name;
            AddProduct(products.ToList());
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public IReadOnlyCollection<Product> Products => _products.ToList().AsReadOnly();

        public void AddProduct(List<Product> products) => _products.ToList().AddRange(products);

        public static Result<Packaging> Create(int id,string name, IEnumerable<Product> products = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<Packaging>("name cannot be null or length > 100");

            var packaging = new Packaging(id, name, products ?? Enumerable.Empty<Product>());

            return Result.Success(packaging);
        }
    }
}
