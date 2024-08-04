using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.Interfaces
{
    public interface IEntryUserEventContext
    {
        public List<EntryUserEvent> GetByEntryEvent(int entryId, int eventId);
    }
}
