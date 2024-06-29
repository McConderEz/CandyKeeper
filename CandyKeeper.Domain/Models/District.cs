using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class District
    {
        public const int MAX_NAME_SIZE = 80;
        private readonly ICollection<Store> _stores = [];

        private District(int id,string name, int cityId,City city, IEnumerable<Store> stores)
        {
            Id = id;
            Name = name;
            CityId = cityId;
            City = city;
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public int CityId { get; }
        public virtual City? City { get; }
        public IReadOnlyCollection<Store> Stores => _stores.ToList().AsReadOnly();

        public void AddStore(List<Store> stores) => _stores.ToList().AddRange(stores);

        public static Result<District> Create(int id, string name, int cityId,City city = null ,IEnumerable<Store> stores = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<District>("name cannot be null or length > 80");

            var district = new District(id,name, cityId, city, stores ?? Enumerable.Empty<Store>());

            return Result.Success(district);
        }
    }
}
