using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using Generator;
using Microsoft.EntityFrameworkCore;


using(var db = new CandyKeeperDbContext())
{
    var list = await new SupplierRepository(db).Get();


    foreach (var item in list)
    {
        Console.WriteLine($"{item.Name}");
        foreach(var suppliers  in item.Stores)
        {
            Console.WriteLine($"\t{suppliers.Name}");
        }
    }
}


//var generator = new NotesGenerator();
//generator.GenAll();