using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Models;

namespace GTRC_Database_API.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
