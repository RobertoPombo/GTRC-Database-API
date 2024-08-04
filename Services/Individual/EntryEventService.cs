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

            if (obj.SignInDate > GlobalValues.DateTimeMaxValue) { obj.SignInDate = GlobalValues.DateTimeMaxValue; isValid = false; }
            else if (obj.SignInDate < GlobalValues.DateTimeMinValue) { obj.SignInDate = GlobalValues.DateTimeMinValue; isValid = false; }
            if (obj.SignInDate < GlobalValues.DateTimeMaxValue && (obj.Entry.RegisterDate > obj.Event.Date || obj.Entry.SignOutDate < obj.Event.Date))
            {
                obj.SignInDate = GlobalValues.DateTimeMaxValue;
                isValid = false;
            }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(EntryEvent? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Entry? entry = null;
            if (obj.Entry is not null) { entry = iEntryContext.GetById(obj.EntryId).Result; };
            if (entry is null)
            {
                List<Entry> list = iEntryContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Entry = list[0]; obj.EntryId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Entry = entry; }
            Event? _event = null;
            if (obj.Event is not null) { _event = iEventContext.GetById(obj.EventId).Result; };
            if (_event is null)
            {
                List<Event> list = iEventContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Event = list[0]; obj.EventId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Event = _event; }

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

            if (obj.Event.SeasonId != seasonId)
            {
                if (listEvent.Count == 0) { obj = null; return false; }
                else
                {
                    startIndexEvent = 0;
                    indexEvent = 0;
                    obj.Event = listEvent[indexEvent];
                    obj.EventId = listEvent[indexEvent].Id;
                    isValidUniqProps = false;
                }
            }

            int startIndexEntry = 0;
            List<int> idListEntry = [];
            List<Entry> listEntry = IEntryContext.GetBySeason(seasonId);
            for (int index = 0; index < listEntry.Count; index++)
            {
                idListEntry.Add(listEntry[index].Id);
                if (listEntry[index].Id == obj.EntryId) { startIndexEntry = index; }
            }
            int indexEntry = startIndexEntry;

            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (indexEvent < idListEvent.Count - 1) { indexEvent++; }
                else { indexEvent = 0; }
                obj.Event = listEvent[indexEvent];
                obj.EventId = listEvent[indexEvent].Id;
                if (indexEvent == startIndexEvent)
                {
                    if (indexEntry < idListEntry.Count - 1) { indexEntry++; }
                    else { indexEntry = 0; }
                    obj.Entry = listEntry[indexEntry];
                    obj.EntryId = listEntry[indexEntry].Id;
                    if (indexEntry == startIndexEntry) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<EntryEvent?> GetTemp() { EntryEvent obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
