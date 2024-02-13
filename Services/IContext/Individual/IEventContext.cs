using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.Interfaces
{
    public interface IEventContext
    {
        public List<Event> GetBySeason(int seasonId);
    }
}
