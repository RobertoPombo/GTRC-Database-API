using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Basics.Models;

namespace GTRC_Database_API.EfcContext
{
    public class EntryUserEventContext(DataContext db) : IEntryUserEventContext
    {
        public List<EntryUserEvent> GetByEntryEvent(int entryId, int eventId)
        {
            return [.. db.EntriesUsersEvents.Where(obj => obj.EventId == eventId).Where(obj => obj.EntryId == entryId)];
        }
    }
}
