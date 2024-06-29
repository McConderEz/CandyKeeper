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

        private ICollection<ProductForSale> _productForSales = [];

        private Product(int id,string name, int productTypeId,ProductType productType ,IEnumerable<ProductForSale> productForSales)
        {
            Id = id;
            Name = name;
            ProductTypeId = productTypeId;
            ProductType = productType;
            AddProductForSale(productForSales.ToList());
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public int ProductTypeId { get; }
        public virtual ProductType? ProductType { get; }

        public IReadOnlyCollection<ProductForSale> ProductForSales => _productForSales.ToList().AsReadOnly();

        public void AddProductForSale(List<ProductForSale> productForSales) => _productForSales.ToList().AddRange(productForSales);

        public static Result<Product> Create(int id,string name, int productTypeId,ProductType productType = null,IEnumerable<ProductForSale> productForSales = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<Product>("name cannot be null or length > 150");

            var product = new Product(id,name, productTypeId,productType ,productForSales ?? Enumerable.Empty<ProductForSale>());

            return Result.Success(product);
        }
    }
}
