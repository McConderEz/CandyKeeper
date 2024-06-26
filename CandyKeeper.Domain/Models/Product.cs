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

        private Product(string name, int productTypeId, int packagingId)
        {
            Name = name;
            ProductTypeId = productTypeId;
            PackagingId = packagingId;
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public int ProductTypeId { get; }
        public virtual ProductType? ProductType { get; }
        public int PackagingId { get; }
        public virtual Packaging? Packaging { get; }

        public IReadOnlyCollection<ProductForSale> ProductsForSale => _productsForSale.ToList().AsReadOnly();

        public void AddProductForSale(ProductForSale productForSale) => _productsForSale.Add(productForSale);

        public static Result<Product> Create(string name, int productTypeId, int packagingId)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<Product>("name cannot be null or length > 150");

            var product = new Product(name, productTypeId, packagingId);

            return Result.Success(product);
        }
    }
}
