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

        private District(int id,string name, int cityId)
        {
            Id = id;
            Name = name;
            CityId = cityId;
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public int CityId { get; }
        public virtual City? City { get; }

        public static Result<District> Create(int id,string name, int cityId)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<District>("name cannot be null or length > 80");

            var district = new District(id,name, cityId);

            return Result.Success(district);
        }
    }
}
