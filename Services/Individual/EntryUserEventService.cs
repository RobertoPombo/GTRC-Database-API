using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EntryUserEventService(IEntryUserEventContext iEntryUserEventContext,
        IBaseContext<User> iUserContext,
        UserService userService,
        IEntryContext IEntryContext,
        IBaseContext<Entry> iEntryContext,
        EntryService entryService,
        IEventContext IEventContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<EntryUserEvent> iBaseContext) : BaseService<EntryUserEvent>(iBaseContext)
    {
        public bool Validate(EntryUserEvent? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.Name3Digits.Length == 3) { obj.Name3Digits = obj.Name3Digits.ToUpper(); } else { obj.Name3Digits = EntryUserEvent.DefaultName3Digits; isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(EntryUserEvent? obj)
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
            User? user = null;
            if (obj.User is not null) { user = iUserContext.GetById(obj.UserId).Result; };
            if (user is null)
            {
                List<User> list = iUserContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.User = list[0]; obj.UserId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.User = user; }
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

            int startIndexEntry = 0;
            List<int> idListEntry = [];
            List<Entry> listEntry = IEntryContext.GetBySeason(seasonId);
            for (int index = 0; index < listEntry.Count; index++)
            {
                idListEntry.Add(listEntry[index].Id);
                if (listEntry[index].Id == obj.EntryId) { startIndexEntry = index; }
            }
            int indexEntry = startIndexEntry;

            int startIndexUser = 0;
            List<int> idListUser = [];
            List<User> listUser = iUserContext.GetAll().Result;
            for (int index = 0; index < listUser.Count; index++)
            {
                idListUser.Add(listUser[index].Id);
                if (listUser[index].Id == obj.UserId) { startIndexUser = index; }
            }
            int indexUser = startIndexUser;

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

            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (indexEvent < idListEvent.Count - 1) { indexEvent++; }
                else { indexEvent = 0; }
                obj.Event = listEvent[indexEvent];
                obj.EventId = listEvent[indexEvent].Id;
                if (indexEvent == startIndexEvent)
                {
                    if (indexUser < idListUser.Count - 1) { indexUser++; }
                    else { indexUser = 0; }
                    obj.User = listUser[indexUser];
                    obj.UserId = listUser[indexUser].Id;
                    if (indexUser == startIndexUser)
                    {
                        if (indexEntry < idListEntry.Count - 1) { indexEntry++; }
                        else { indexEntry = 0; }
                        obj.Entry = listEntry[indexEntry];
                        obj.EntryId = listEntry[indexEntry].Id;
                        if (indexEntry == startIndexEntry) { obj = null; return false; }
                    }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<EntryUserEvent?> GetTemp() { EntryUserEvent obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<List<EntryUserEvent>> GetNames3Digits(int eventId)
        {
            List<EntryUserEvent> listEues = await GetChildObjects(typeof(Event), eventId);
            bool allUnique = false;
            int number = 0;
            foreach (EntryUserEvent eue in listEues) { eue.Name3Digits = userService.GetName3DigitsOptions(eue.User)[0]; }
            while (!allUnique)
            {
                allUnique = true;
                foreach (EntryUserEvent eue in listEues)
                {
                    List<EntryUserEvent> identicalN3D = [];
                    string currentN3D = eue.Name3Digits;
                    foreach (EntryUserEvent eue2 in listEues) { if (currentN3D == eue2.Name3Digits) { identicalN3D.Add(eue2); } }
                    if (identicalN3D.Count > 1)
                    {
                        int lvlsMax = -1;
                        List<EntryUserEvent> identicalN3D_0 = [];
                        List<EntryUserEvent> identicalN3D_1 = [];
                        foreach (EntryUserEvent eue2 in identicalN3D)
                        {
                            List<string> eue2Name3DigitsOptions = userService.GetName3DigitsOptions(eue2.User);
                            if (eue2Name3DigitsOptions.IndexOf(currentN3D) == 0) { identicalN3D_0.Add(eue2); }
                            int lvlsMaxTemp = eue2Name3DigitsOptions.Count - eue2Name3DigitsOptions.IndexOf(currentN3D);
                            if (lvlsMaxTemp > lvlsMax) { lvlsMax = lvlsMaxTemp; identicalN3D_1 = [eue2]; }
                        }
                        if (identicalN3D_0.Count > 0) { identicalN3D = identicalN3D_0; } else { identicalN3D = identicalN3D_1; }
                        foreach (EntryUserEvent eue2 in identicalN3D)
                        {
                            List<string> eue2Name3DigitsOptions = userService.GetName3DigitsOptions(eue2.User);
                            int currentLvl = eue2Name3DigitsOptions.IndexOf(currentN3D) + 1;
                            int lvlMax = eue2Name3DigitsOptions.Count;
                            if (currentLvl == lvlMax) { eue2.Name3Digits = number.ToString(); number++; }
                            else { eue2.Name3Digits = eue2Name3DigitsOptions[currentLvl]; }
                        }
                        allUnique = false;
                        break;
                    }
                }
            }
            foreach (EntryUserEvent eue in listEues)
            {
                if (int.TryParse(eue.Name3Digits, out number))
                {
                    List<string> eueName3DigitsOptions = userService.GetName3DigitsOptions(eue.User);
                    eue.Name3Digits = eueName3DigitsOptions[0];
                }
            }
            return listEues;
        }
    }
}
