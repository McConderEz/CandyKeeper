using CandyKeeper.DAL;
using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using CandyKeeper.DAL.Entities;

namespace Generator
{
    public class NotesGenerator
    {
        private readonly CandyKeeperDbContext _dbContext = new CandyKeeperDbContext();
        private readonly Parser _parser = new Parser();

        private const string PATH = "C:\\Users\\rusta\\source\\repos\\CandyKeeper\\Generator\\Dataset\\";

        public void GenAll()
        {
            //GenCity();
            //GenDistrict();
            //GenOwnershipType();
            //GenProductType();
            GenPackaging();
        }



        public void GenCity()
        {
            var list = _parser.Parse(PATH + "City.txt");
            for (var i = 0; i < list.Count; i++)
            {
                _dbContext.Cities.Add(new CityEntity { Name = list[i] });
            }
            _dbContext.SaveChanges();
        }


        public void GenDistrict()
        {
            var list = _parser.ParseCSV(PATH + "District.csv");
            for (var i = 0; i < list.Count; i++)
            {
                var city = _dbContext.Cities.First(c => c.Name.Equals(list[i].Item1));
                for (var j = 0; j < list[i].Item2.Count(); j++)
                {
                    _dbContext.Districts.Add(new DistrictEntity { Name = list[i].Item2[j], CityId = city.Id });
                }
            }
            _dbContext.SaveChanges();
        }

        public void GenOwnershipType()
        {
            var list = _parser.Parse(PATH + "OwnershipType.txt");
            for (var i = 0; i < list.Count; i++)
            {
                _dbContext.OwnershipTypes.Add(new OwnershipTypeEntity { Name = list[i] });
            }
            _dbContext.SaveChanges();
        }


        public void GenProductType()
        {
            var list = _parser.Parse(PATH + "ProductType.txt");
            for (var i = 0; i < list.Count; i++)
            {
                _dbContext.ProductTypes.Add(new ProductTypeEntity { Name = list[i] });
            }
            _dbContext.SaveChanges();
        }

        public void GenPackaging()
        {
            var list = _parser.Parse(PATH + "Packaging.txt");
            for (var i = 0; i < list.Count; i++)
            {
                _dbContext.Packaging.Add(new PackagingEntity { Name = list[i] });
            }
            _dbContext.SaveChanges();
        }

    }
    
}
