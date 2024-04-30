using Microsoft.AspNetCore.Hosting.Server;
using System.Diagnostics.Metrics;

namespace Skelbimu_sistema.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

		public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        //public DbSet<Category> Categories { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Suspension> Suspensions { get; set; }
        public DbSet<Wish> UserWishes { get; set; }
    }
}
