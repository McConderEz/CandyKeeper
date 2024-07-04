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

        private List<ProductForSale> _productForSales = [];

        private Packaging(int id,string name, IEnumerable<ProductForSale> productForSales)
        {
            Id = id;
            Name = name;
            AddProductForSales(productForSales.ToList());
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public IReadOnlyCollection<ProductForSale> ProductForSales => _productForSales.AsReadOnly();

        public void AddProductForSales(List<ProductForSale> productForSales) => _productForSales.AddRange(productForSales);

        public static Result<Packaging> Create(int id,string name, IEnumerable<ProductForSale> productForSales = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<Packaging>("name cannot be null or length > 100");

            var packaging = new Packaging(id, name, productForSales ?? Enumerable.Empty<ProductForSale>());

            return Result.Success(packaging);
        }
    }
}
