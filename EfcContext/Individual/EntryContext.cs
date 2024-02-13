using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Basics.Models;

namespace GTRC_Database_API.EfcContext
{
    public class EntryContext(DataContext db) : IEntryContext
    {
        public List<Entry> GetBySeason(int seasonId)
        {
            return [.. db.Entries.Where(obj => obj.SeasonId == seasonId)];
        }
    }
}
