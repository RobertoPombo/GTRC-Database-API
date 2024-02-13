using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EntryEventService(IEntryEventContext iEntryEventContext,
        IEntryContext IEntryContext,
        IBaseContext<Entry> iEntryContext,
        IEventContext IEventContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<EntryEvent> iBaseContext) : BaseService<EntryEvent>(iBaseContext)
    {
        public bool Validate(EntryEvent? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Entry? entry = null;
            if (obj.Entry is not null) { entry = iEntryContext.GetById(obj.EntryId).Result; };
            if (entry is null)
            {
                List<Entry> list = iEntryContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Entry = list[0]; obj.EntryId = list[0].Id; isValid = false; }
            }
            else { obj.Entry = entry; }
            Event? _event = null;
            if (obj.Event is not null) { _event = iEventContext.GetById(obj.EventId).Result; };
            if (_event is null)
            {
                List<Event> list = iEventContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Event = list[0]; obj.EventId = list[0].Id; isValid = false; }
            }
            else { obj.Event = _event; }
            if (obj.SignInDate > GlobalValues.DateTimeMaxValue) { obj.SignInDate = GlobalValues.DateTimeMaxValue; isValid = false; }
            else if (obj.SignInDate < GlobalValues.DateTimeMinValue) { obj.SignInDate = GlobalValues.DateTimeMinValue; isValid = false; }
            if (obj.Entry.RegisterDate > obj.Event.Date || obj.Entry.SignOutDate < obj.Event.Date) { obj.SignInDate = GlobalValues.DateTimeMaxValue; isValid = false; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(EntryEvent? obj)
        {
            bool isAvailable = true;
            if (obj is null) { return false; }

            int seasonId = obj.Entry.SeasonId;
            int startIndexEvent = 0;
            List<int> idListEvent = [];
            List<Event> listEvent = IEventContext.GetBySeason(seasonId);
            for (int index = 0; index < listEvent.Count; index++)
            {
                idListEvent.Add(listEvent[index].Id);
                if (listEvent[index].Id == obj.EventId) { startIndexEvent = index; }
            }
            int indexEvent = startIndexEvent;

            while (!await IsUnique(obj))
            {
                isAvailable = false;
                if (indexEvent < idListEvent.Count - 1)
                {
                    indexEvent++;
                    obj.Event = listEvent[indexEvent];
                    obj.EventId = listEvent[indexEvent].Id;
                }
                else { indexEvent = 0; }
                if (indexEvent == startIndexEvent)
                {
                    int startIndexEntry = 0;
                    List<int> idListEntry = [];
                    List<Entry> listEntry = IEntryContext.GetBySeason(seasonId);
                    for (int index = 0; index < listEntry.Count; index++)
                    {
                        idListEntry.Add(listEntry[index].Id);
                        if (listEntry[index].Id == obj.EntryId) { startIndexEntry = index; }
                    }
                    int indexEntry = startIndexEntry;

                    if (indexEntry < idListEntry.Count - 1)
                    {
                        indexEntry++;
                        obj.Entry = listEntry[indexEntry];
                        obj.EntryId = listEntry[indexEntry].Id;
                    }
                    else { indexEntry = 0; }
                    if (indexEntry == startIndexEntry) { obj = null; return false; }
                }
            }

            return isAvailable;
        }

        public async Task<EntryEvent?> GetTemp() { EntryEvent obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
