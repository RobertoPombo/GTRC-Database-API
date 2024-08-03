using Microsoft.EntityFrameworkCore;

using GTRC_Basics.Models;

namespace GTRC_Database_API.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Color> Colors { get; set; }
        public DbSet<Sim> Simulations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Carclass> Carclasses { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UsersRoles { get; set; }
        public DbSet<UserDatetime> UsersDatetimes { get; set; }
        public DbSet<Bop> Bops { get; set; }
        public DbSet<BopTrackCar> BopsTracksCars { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<SeasonCarclass> SeasonsCarclasses { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationUser> OrganizationsUsers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<EntryDatetime> EntriesDatetimes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCarclass> EventsCarclasses { get; set; }
        public DbSet<EventCar> EventsCars { get; set; }
        public DbSet<EntryEvent> EntriesEvents { get; set; }
        public DbSet<EntryUserEvent> EntriesUsersEvents { get; set; }
        public DbSet<Pointssystem> Pointssystems { get; set; }
        public DbSet<PointssystemPosition> PointssystemsPositions { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Resultsfile> Resultsfiles { get; set; }
        public DbSet<Lap> Laps { get; set; }
        public DbSet<Leaderboardline> Leaderboardlines { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<IncidentEntry> IncidentsEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs) { fk.DeleteBehavior = DeleteBehavior.NoAction; }

            base.OnModelCreating(modelBuilder);
        }
    }
}
