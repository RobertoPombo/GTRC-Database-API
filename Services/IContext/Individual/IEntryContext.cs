using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.Interfaces
{
    public interface IEntryContext
    {
        public List<Entry> GetBySeason(int seasonId);
    }
}
