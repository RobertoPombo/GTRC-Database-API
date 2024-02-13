using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Basics.Models;

namespace GTRC_Database_API.EfcContext
{
    public class EventContext(DataContext db) : IEventContext
    {
        public List<Event> GetBySeason(int seasonId)
        {
            return [.. db.Events.Where(obj => obj.SeasonId == seasonId)];
        }
    }
}
