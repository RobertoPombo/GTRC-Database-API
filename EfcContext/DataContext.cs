using Microsoft.EntityFrameworkCore;

using GTRC_Basics.Models;

namespace GTRC_Database_API.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Color> Colors { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<Sim> Simulations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Carclass> Carclasses { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UsersRoles { get; set; }
        public DbSet<Bop> Bops { get; set; }
        public DbSet<BopTrackCar> BopsTracksCars { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<SeasonCarclass> SeasonsCarclasses { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationUser> OrganizationsUsers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCarclass> EventsCarclasses { get; set; }
        public DbSet<EventCar> EventsCars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
