using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Models
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options) { }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemClient> ItemClients { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SerialNumber> SerialNumbers { get; set; }
        public DbSet<Client> Clients { get; set; }
    }

}
