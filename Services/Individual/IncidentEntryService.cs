using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class IncidentEntryService(IIncidentEntryContext iIncidentEntryContext,
        IEntryContext IEntryContext,
        IEntryUserEventContext IEntryUserEventContext,
        IIncidentContext IIncidentContext,
        IBaseContext<Incident> iIncidentContext,
        IBaseContext<Entry> iEntryContext,
        IBaseContext<User> iUserContext,
        UserService userService,
        IBaseContext<IncidentEntry> iBaseContext) : BaseService<IncidentEntry>(iBaseContext)
    {
        public bool Validate(IncidentEntry? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            User? user = null;
            if (obj.User is not null) { user = iUserContext.GetById(obj.UserId).Result; };
            if (user is null)
            {
                List<User> list = iUserContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.User = list[0]; obj.UserId = list[0].Id; isValid = false; }
            }
            else { obj.User = user; }
            Incident? incident = iIncidentContext.GetById(obj.IncidentId).Result;
            List<User> listUser = userService.GetByEntryEvent(obj.EntryId, incident?.Session.EventId ?? GlobalValues.NoId).Result;
            if (listUser.Count == 0) { obj = null; return false; }
            else
            {
                bool userIsInList = false;
                foreach (User _user in listUser) { if (_user.Id == obj.UserId) { userIsInList = true; break; } }
                if (!userIsInList) { obj.User = listUser[0]; obj.UserId = listUser[0].Id; isValid = false; }
            }
            if (obj.IsAtFault)
            {
                IncidentEntry? incidentEntryAtFault = GetIncidentEntryAtFault(obj.IncidentId).Result;
                if (incidentEntryAtFault is not null && incidentEntryAtFault.EntryId != obj.EntryId) { obj.IsAtFault = false; isValid = false; }
            }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(IncidentEntry? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Incident? incident = null;
            if (obj.Incident is not null) { incident = iIncidentContext.GetById(obj.IncidentId).Result; };
            if (incident is null)
            {
                List<Incident> list = iIncidentContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Incident = list[0]; obj.IncidentId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Incident = incident; }
            Entry? entry = null;
            if (obj.Entry is not null) { entry = iEntryContext.GetById(obj.EntryId).Result; };
            if (entry is null)
            {
                List<Entry> list = iEntryContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Entry = list[0]; obj.EntryId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Entry = entry; }

            int seasonId = obj.Incident.Session.Event.SeasonId;

            int startIndexIncident = 0;
            List<int> idListIncident = [];
            List<Incident> listIncident = IIncidentContext.GetBySeason(seasonId);
            for (int index = 0; index < listIncident.Count; index++)
            {
                idListIncident.Add(listIncident[index].Id);
                if (listIncident[index].Id == obj.IncidentId) { startIndexIncident = index; }
            }
            int indexIncident = startIndexIncident;

            int startIndexEntry = 0;
            List<int> idListEntry = [];
            List<Entry> listEntry = IEntryContext.GetBySeason(seasonId);
            for (int index = 0; index < listEntry.Count; index++)
            {
                idListEntry.Add(listEntry[index].Id);
                if (listEntry[index].Id == obj.EntryId) { startIndexEntry = index; }
            }
            int indexEntry = startIndexEntry;

            if (obj.Entry.SeasonId != seasonId)
            {
                if (listEntry.Count == 0) { obj = null; return false; }
                else
                {
                    startIndexEntry = 0;
                    indexEntry = 0;
                    obj.Entry = listEntry[indexEntry];
                    obj.EntryId = listEntry[indexEntry].Id;
                    isValidUniqProps = false;
                }
            }

            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (indexEntry < idListEntry.Count - 1) { indexEntry++; }
                else { indexEntry = 0; }
                obj.Entry = listEntry[indexEntry];
                obj.EntryId = listEntry[indexEntry].Id;
                if (indexEntry == startIndexEntry)
                {
                    if (indexIncident < idListIncident.Count - 1) { indexIncident++; }
                    else { indexIncident = 0; }
                    obj.Incident = listIncident[indexIncident];
                    obj.IncidentId = listIncident[indexIncident].Id;
                    if (indexIncident == startIndexIncident) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<IncidentEntry?> GetTemp() { IncidentEntry obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<IncidentEntry?> GetIncidentEntryAtFault(int incidentId)
        {
            IncidentEntry? incidentEntryAtFault = null;
            List<IncidentEntry> listIncidentEntries = await GetChildObjects(typeof(Incident), incidentId);
            foreach (IncidentEntry incidentEntry in listIncidentEntries)
            {
                if (incidentEntry.IsAtFault) { incidentEntryAtFault = incidentEntry; break; }
            }
            return incidentEntryAtFault;
        }
    }
}
