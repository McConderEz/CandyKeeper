using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CandyKeeper.DAL.Repositories
{
    public class PackagingRepository : IPackagingRepository
    {
        private readonly CandyKeeperDbContext _context;

        //TODO: Изменить Packaging: Расфософка должна быть у продукта на продажу, так как один продукт может продаваться и поштучно, и в кг, и в коробках

        public PackagingRepository(CandyKeeperDbContext context)
        {
            _context = context;
        }

        public async Task<List<Packaging>> Get()
        {
            var packagingEntity = await _context.Packaging
                .AsNoTracking()
                .Include(p => p.Products)
                .ToListAsync();

            var packaging = packagingEntity
                .Select(p => Packaging.Create(p.Id, p.Name, p.Products.Select(p => Product.Create(p.Id, p.Name, p.ProductTypeId, p.PackagingId).Value)).Value)
                .ToList();

            return packaging;
        }

        public async Task<Packaging> GetById(int id)
        {
            var packagingEntity = await _context.Packaging
                                           .Include(p => p.Products)
                                           .FirstOrDefaultAsync(c => c.Id == id);

            if (packagingEntity == null)
                throw new Exception("Packaging entity is null");

            var packaging = Packaging.Create(packagingEntity.Id, packagingEntity.Name, packagingEntity.Products.Select(p => Product.Create(p.Id, p.Name, p.ProductTypeId, p.PackagingId).Value)).Value;

            return packaging;
        }

        public async Task Create(Packaging packaging)
        {
            var packagingEntity = new PackagingEntity
            {
                Name = packaging.Name,
            };

            await _context.AddAsync(packagingEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, string name)
        {
            await _context.Packaging
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p
                                   .SetProperty(p => p.Name, name));

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Packaging
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
        }
    }
}
