using GTRC_Database_API.Data;
using GTRC_Database_API.Models;
using GTRC_Database_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GTRC_Database_API.EfcContext
{
    public class CarContext(DataContext db) : ICarContext
    {
        public async Task SaveChanges() { await db.SaveChangesAsync(); }
        public async Task<List<Car>> GetAll() { List<Car> list = await db.Cars.ToListAsync(); return list; }
    }
}
