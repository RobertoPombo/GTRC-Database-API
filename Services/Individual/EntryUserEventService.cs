using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EntryUserEventService(IEntryUserEventContext iEntryUserEventContext,
        IBaseContext<User> iUserContext,
        IEntryContext IEntryContext,
        IBaseContext<Entry> iEntryContext,
        IEventContext IEventContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<EntryUserEvent> iBaseContext) : BaseService<EntryUserEvent>(iBaseContext)
    {
        public bool Validate(EntryUserEvent? obj)
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
            User? user = null;
            if (obj.User is not null) { user = iUserContext.GetById(obj.UserId).Result; };
            if (user is null)
            {
                List<User> list = iUserContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.User = list[0]; obj.UserId = list[0].Id; isValid = false; }
            }
            else { obj.User = user; }
            Event? _event = null;
            if (obj.Event is not null) { _event = iEventContext.GetById(obj.EventId).Result; };
            if (_event is null)
            {
                List<Event> list = iEventContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Event = list[0]; obj.EventId = list[0].Id; isValid = false; }
            }
            else { obj.Event = _event; }
            if (obj.Name3Digits.Length == 3) { obj.Name3Digits = obj.Name3Digits.ToUpper(); } else { obj.Name3Digits = EntryUserEvent.DefaultName3Digits; isValid = false; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(EntryUserEvent? obj)
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
                    int startIndexUser = 0;
                    List<int> idListUser = [];
                    List<User> listUser = iUserContext.GetAll().Result;
                    for (int index = 0; index < listUser.Count; index++)
                    {
                        idListUser.Add(listUser[index].Id);
                        if (listUser[index].Id == obj.UserId) { startIndexUser = index; }
                    }
                    int indexUser = startIndexUser;

                    if (indexUser < idListUser.Count - 1)
                    {
                        indexUser++;
                        obj.User = listUser[indexUser];
                        obj.UserId = listUser[indexUser].Id;
                    }
                    else { indexUser = 0; }
                    if (indexUser == startIndexUser)
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
            }

            return isAvailable;
        }

        public async Task<EntryUserEvent?> GetTemp() { EntryUserEvent obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
