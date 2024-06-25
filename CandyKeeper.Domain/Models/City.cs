using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CandyKeeper.Domain.Models
{
    public class City
    {
        public const int MAX_NAME_LENGTH = 80;

        private readonly ICollection<District> _districts = [];
        private City(string name)
        {
            Name = name;
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public IReadOnlyCollection<District> Districts => _districts.ToList().AsReadOnly();

        public void AddDistrict(District district) => _districts.Add(district);

        public static Result<City> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 80)
                return Result.Failure<City>("name cannot be null or length > 80");

            var city = new City(name);

            return Result.Success(city);
        }

    }
}
