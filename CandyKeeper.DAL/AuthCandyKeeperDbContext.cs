using CandyKeeper.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CandyKeeper.DAL;

public class AuthCandyKeeperDbContext: DbContext
{
    private readonly IConfiguration _configuration;
        
    public AuthCandyKeeperDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        Database.EnsureCreated();
    }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("AuthConnection"));
    }
        
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CandyKeeperDbContext).Assembly);
    }
    
    public DbSet<UserEntity> Users { get; set; } = null!;
}