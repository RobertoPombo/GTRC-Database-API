using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.Interfaces
{
    public interface IIncidentContext
    {
        public List<Incident> GetBySeason(int seasonId);
        public List<Incident> GetByEvent(int eventId);
        public List<Incident> GetBySession(int sessionId);
    }
}
