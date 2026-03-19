using FurnitureShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShopApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Furniture> Furnitures { get; set; }
        public DbSet<User> Users { get; set; }
    }
}