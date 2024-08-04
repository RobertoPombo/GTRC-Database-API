using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Basics.Models;

namespace GTRC_Database_API.EfcContext
{
    public class SessionContext(DataContext db) : ISessionContext
    {
        public List<Session> GetBySeason(int seasonId)
        {
            return [.. db.Sessions.Where(obj => obj.Event.SeasonId == seasonId)];
        }

        public List<Session> GetByEvent(int eventId)
        {
            return [.. db.Sessions.Where(obj => obj.EventId == eventId)];
        }
    }
}
