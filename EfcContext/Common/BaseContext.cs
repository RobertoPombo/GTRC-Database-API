using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Basics.Models.Common;

namespace GTRC_Database_API.EfcContext
{
    public class BaseContext<ModelType>(DataContext db) : IBaseContext<ModelType> where ModelType : class, IBaseModel, new()
    {
        public async Task SaveChanges() { await db.SaveChangesAsync(); }

        public async Task<List<ModelType>> GetAll() { List<ModelType> list = await db.Set<ModelType>().ToListAsync(); return list; }

        public async Task<ModelType?> GetById(int id) { return await db.Set<ModelType>().FindAsync(id); }

        public async Task Add(ModelType obj) { db.Set<ModelType>().Add(obj); await SaveChanges(); }

        public async Task Delete(ModelType obj) { db.Remove(obj); await SaveChanges(); }

        public async Task Update(ModelType obj) { db.Update(obj); await SaveChanges(); }
    }
}
