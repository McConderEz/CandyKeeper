using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using Generator;
using Microsoft.EntityFrameworkCore;


using(var db = new CandyKeeperDbContext())
{
    var list = await new CityRepository(db).Get();


    foreach (var city in list)
    {
        Console.WriteLine($"{city.Name}");
        foreach(var suppliers  in city.Suppliers)
        {
            Console.WriteLine($"\t{suppliers.Name}");
        }
    }
}


//var generator = new NotesGenerator();
//generator.GenAll();