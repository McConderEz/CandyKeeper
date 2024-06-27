using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class Product
    {
        
        public const int MAX_NAME_SIZE = 150;

        private ICollection<ProductForSale> _productsForSale = [];

        private Product(int id,string name, int productTypeId, int packagingId, IEnumerable<ProductForSale> productForSales)
        {
            Id = id;
            Name = name;
            ProductTypeId = productTypeId;
            PackagingId = packagingId;
            AddProductForSale(productForSales.ToList());
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public int ProductTypeId { get; }
        public virtual ProductType? ProductType { get; }
        public int PackagingId { get; }
        public virtual Packaging? Packaging { get; }

        public IReadOnlyCollection<ProductForSale> ProductsForSale => _productsForSale.ToList().AsReadOnly();

        public void AddProductForSale(List<ProductForSale> productForSales) => _productsForSale.ToList().AddRange(productForSales);

        public static Result<Product> Create(int id,string name, int productTypeId, int packagingId, IEnumerable<ProductForSale> productForSales = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<Product>("name cannot be null or length > 150");

            var product = new Product(id,name, productTypeId, packagingId, productForSales ?? Enumerable.Empty<ProductForSale>());

            return Result.Success(product);
        }
    }
}
