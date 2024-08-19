using System.Net;

using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EntryEventService(IEntryEventContext iEntryEventContext,
        IEntryContext IEntryContext,
        IEventContext IEventContext,
        IBaseContext<Entry> iEntryContext,
        IBaseContext<Event> iEventContext,
        EventService eventService,
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

        public async Task<(HttpStatusCode, EntryEvent?)> GetAnyByUniqProps(EntryEventUniqPropsDto0 objDto)
        {
            UniqPropsDto<EntryEvent> uniqDto = new() { Dto = objDto };
            EntryEvent? obj = await GetByUniqProps(uniqDto);
            if (obj is not null) { return (HttpStatusCode.OK, obj); }
            else
            {
                Entry? entry = await iEntryContext.GetById(objDto.EntryId);
                Event? _event = await iEventContext.GetById(objDto.EventId);
                if (entry is null || _event is null) { return (HttpStatusCode.NotFound, null); }
                else
                {
                    DateTime signInDate = GlobalValues.DateTimeMaxValue;
                    if (EntryFullDto.GetRegisterState(entry) && entry.IsPermanent) { signInDate = GlobalValues.DateTimeMinValue; }
                    EntryEvent newObj = new()
                    {
                        EntryId = entry.Id,
                        EventId = _event.Id,
                        SignInDate = signInDate,
                        IsPointScorer = entry.IsPointScorer
                    };
                    await ValidateUniqProps(newObj);
                    if (newObj is not null) { return (HttpStatusCode.OK, newObj); }
                    else { return (HttpStatusCode.NotAcceptable, newObj); }
                }
            }
        }

        public async Task<byte> GetSignOutsCount(Entry entry, Event nextEvent)
        {
            byte count = 0;
            List<Event> listEvents = Scripts.SortByDate(IEventContext.GetBySeason(entry.SeasonId));
            foreach (Event _event in listEvents)
            {
                if (entry.RegisterDate < _event.Date)
                {
                    if (_event.Date >= nextEvent.Date) { return count; }
                    if (_event.Date > entry.SignOutDate) { return count; }
                    (HttpStatusCode status, EntryEvent? entryEvent) = await GetAnyByUniqProps(new() { EntryId = entry.Id, EventId = _event.Id });
                    if (status == HttpStatusCode.OK && entryEvent is not null && !EntryEventFullDto.GetSignInState(entryEvent))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public async Task<byte> GetNoShowsCount(Entry entry, Event nextEvent)
        {
            byte count = 0;
            List<Event> listEvents = Scripts.SortByDate(IEventContext.GetBySeason(entry.SeasonId));
            for (int eventNr = 0; eventNr < listEvents.Count; eventNr++)
            {
                if (entry.RegisterDate < listEvents[eventNr].Date)
                {
                    if (listEvents[eventNr].Date >= nextEvent.Date) { return count; }
                    if (listEvents[eventNr].Date > entry.SignOutDate) { return count; }
                    (HttpStatusCode status, EntryEvent? entryEvent) = await GetAnyByUniqProps(new() { EntryId = entry.Id, EventId = listEvents[eventNr].Id });
                    if (status == HttpStatusCode.OK && entryEvent is not null && EntryEventFullDto.GetSignInState(entryEvent) && entryEvent.IsOnEntrylist && !entryEvent.DidAttend &&
                        await eventService.GetIsOver(listEvents[eventNr]))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
