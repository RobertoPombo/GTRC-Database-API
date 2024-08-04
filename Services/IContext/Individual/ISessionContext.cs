using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.Interfaces
{
    public interface ISessionContext
    {
        public List<Session> GetBySeason(int seasonId);
        public List<Session> GetByEvent(int eventId);
    }
}
