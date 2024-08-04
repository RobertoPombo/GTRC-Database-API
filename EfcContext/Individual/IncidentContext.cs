using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Basics.Models;

namespace GTRC_Database_API.EfcContext
{
    public class IncidentContext(DataContext db) : IIncidentContext
    {
        public List<Incident> GetBySeason(int seasonId)
        {
            return [.. db.Incidents.Where(obj => obj.Session.Event.SeasonId == seasonId)];
        }

        public List<Incident> GetByEvent(int eventId)
        {
            return [.. db.Incidents.Where(obj => obj.Session.EventId == eventId)];
        }

        public List<Incident> GetBySession(int sessionId)
        {
            return [.. db.Incidents.Where(obj => obj.SessionId == sessionId)];
        }
    }
}
