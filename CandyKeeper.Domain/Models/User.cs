using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Domain.Models
{
    public class User
    {
        public const int MAX_NAME_SIZE = 100;

        private User(int id,string name, string passwordHashed)
        {
            Name = name;
            PasswordHashed= passwordHashed;
        }

        public int Id { get; }
        public string Name { get; } = string.Empty;
        public string PasswordHashed { get; } = string.Empty;


        public static Result<User> Create(int id,string name, string passwordHashed)
        {
            if (string.IsNullOrEmpty(name) || name.Length > MAX_NAME_SIZE)
                return Result.Failure<User>("Name is null or empty");

            if (string.IsNullOrEmpty(passwordHashed))
                return Result.Failure<User>("PasswordHashed is null or empty");

            var user = new User(id, name, passwordHashed);

            return Result.Success(user);
        }
    }
}
