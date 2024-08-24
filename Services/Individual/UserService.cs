using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class UserService(IUserContext iUserContext,
        BaseService<Entry> entryService,
        BaseService<Event> eventService,
        BaseService<EntryUserEvent> entryUserEventService,
        BaseService<OrganizationUser> organizationUserService,
        IBaseContext<User> iBaseContext) : BaseService<User>(iBaseContext)
    {
        public bool Validate(User? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            obj.FirstName = Scripts.RemoveSpaceStartEnd(obj.FirstName);
            if (obj.FirstName == string.Empty) { obj.FirstName = nameof(obj.FirstName); isValid = false; }
            obj.LastName = Scripts.RemoveSpaceStartEnd(obj.LastName);
            if (obj.LastName == string.Empty) { obj.LastName = nameof(obj.LastName); isValid = false; }
            if (obj.RegisterDate > DateTime.UtcNow || obj.RegisterDate < GlobalValues.DateTimeMinValue) { obj.RegisterDate = DateTime.UtcNow; isValid = false; }
            if (obj.BanDate > GlobalValues.DateTimeMaxValue || obj.BanDate < obj.RegisterDate) { obj.BanDate = GlobalValues.DateTimeMaxValue; isValid = false; }
            if (obj.Name3Digits.Length == 3) { obj.Name3Digits = obj.Name3Digits.ToUpper(); } else { obj.Name3Digits = string.Empty; }
            obj.NickName = Scripts.RemoveSpaceStartEnd(obj.NickName);
            if (obj.NickName == string.Empty) { obj.NickName = nameof(obj.NickName); isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(User? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            if (!Scripts.IsValidSteamId(obj.SteamId) && obj.SteamId != GlobalValues.NoSteamId) { obj.SteamId = GlobalValues.NoSteamId; isValidUniqProps = false; }
            if (!Scripts.IsValidDiscordId(obj.DiscordId) && obj.DiscordId != GlobalValues.NoDiscordId) { obj.DiscordId = GlobalValues.NoDiscordId; isValidUniqProps = false; }

            ulong? startValue = obj.SteamId;
            while (!await IsUnique(obj, 0) && obj.SteamId != GlobalValues.NoSteamId)
            {
                isValidUniqProps = false;
                if (obj.SteamId < GlobalValues.MaxSteamId) { obj.SteamId++; } else { obj.SteamId = GlobalValues.MinSteamId; }
                if (obj.SteamId == startValue) { obj.SteamId = GlobalValues.NoSteamId; }
            }

            startValue = obj.DiscordId;
            while (!await IsUnique(obj, 1) && obj.DiscordId != GlobalValues.NoDiscordId)
            {
                isValidUniqProps = false;
                if (obj.DiscordId < GlobalValues.MaxDiscordId) { obj.DiscordId++; } else { obj.DiscordId = GlobalValues.MinDiscordId; }
                if (obj.DiscordId == startValue) { obj.DiscordId = GlobalValues.NoDiscordId; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<User?> GetTemp() { User obj = new(); await ValidateUniqProps(obj); return obj; }

        public List<string> GetName3DigitsOptions(User obj)
        {
            List<string> listFirstNames;
            List<string> listLastNames;
            List<string> tempListN3D = [];
            listFirstNames = FilterLetters4N3D(obj.FirstName);
            listLastNames = FilterLetters4N3D(obj.LastName);
            List<string> listAllNames = [.. listFirstNames, .. listLastNames];
            tempListN3D = AddN3D(tempListN3D, obj.Name3Digits);
            string n3D = string.Empty;
            foreach (string _name in listLastNames) { n3D += _name[0]; }
            tempListN3D = AddN3D(tempListN3D, n3D);
            n3D = string.Empty;
            foreach (string _name in listAllNames) { n3D += _name[0]; }
            tempListN3D = AddN3D(tempListN3D, n3D);
            foreach (string _name in listLastNames) { tempListN3D = AddN3D(tempListN3D, _name); }
            n3D = string.Empty;
            foreach (string _name in listLastNames) { n3D += _name; }
            tempListN3D = AddN3D(tempListN3D, n3D);
            foreach (string _fname in listFirstNames)
            {
                n3D = _fname[..1];
                foreach (string _name in listLastNames) { n3D += _name; }
                tempListN3D = AddN3D(tempListN3D, n3D);
            }
            n3D = string.Empty;
            foreach (string _name in listLastNames)
            {
                n3D += _name[..1] + Scripts.StrRemoveVocals(_name[1..]);
            }
            tempListN3D = AddN3D(tempListN3D, n3D);
            foreach (string _fname in listFirstNames)
            {
                n3D = _fname[..1];
                foreach (string _name in listLastNames)
                {
                    n3D += _name[..1] + Scripts.StrRemoveVocals(_name[1..]);
                }
                tempListN3D = AddN3D(tempListN3D, n3D);
            }
            foreach (string _fname in listFirstNames) { tempListN3D = AddN3D(tempListN3D, _fname); }
            n3D = string.Empty;
            foreach (string _name in listLastNames)
            {
                n3D += _name[..1] + Scripts.StrRemoveVocals(_name[1..]);
            }
            if (n3D.Length > 2)
            {
                for (int charNr1 = 1; charNr1 < n3D.Length - 1; charNr1++)
                {
                    for (int charNr2 = charNr1 + 1; charNr2 < n3D.Length; charNr2++)
                    {
                        tempListN3D = AddN3D(tempListN3D, n3D[..1] + n3D[charNr1] + n3D[charNr2]);
                    }
                }
            }
            foreach (string _fname in listFirstNames)
            {
                n3D = _fname[..1];
                foreach (string _name in listLastNames)
                {
                    n3D += _name[..1] + Scripts.StrRemoveVocals(_name[1..]);
                }
                if (n3D.Length > 2)
                {
                    for (int charNr = 2; charNr < n3D.Length; charNr++)
                    {
                        tempListN3D = AddN3D(tempListN3D, n3D[..2] + n3D[charNr]);
                    }
                }
            }
            n3D = string.Empty;
            foreach (string _name in listLastNames) { n3D += Scripts.StrRemoveVocals(_name[1..]); }
            if (n3D.Length > 2)
            {
                for (int charNr1 = 1; charNr1 < n3D.Length - 1; charNr1++)
                {
                    for (int charNr2 = charNr1 + 1; charNr2 < n3D.Length; charNr2++)
                    {
                        tempListN3D = AddN3D(tempListN3D, n3D[..1] + n3D[..1] + n3D[charNr2]);
                    }
                }
            }
            foreach (string _fname in listFirstNames)
            {
                n3D = _fname[..1];
                foreach (string _name in listLastNames) { n3D += _name; }
                if (n3D.Length > 2)
                {
                    for (int charNr = 2; charNr < n3D.Length; charNr++)
                    {
                        tempListN3D = AddN3D(tempListN3D, n3D[..2] + n3D[charNr]);
                    }
                }
            }
            n3D = string.Empty;
            foreach (string _name in listAllNames) { n3D += _name; }
            n3D += "XXX";
            return AddN3D(tempListN3D, n3D);
        }

        public static List<string> AddN3D(List<string> tempListN3D, string n3D)
        {
            if (n3D.Length > 2 && !tempListN3D.Contains(n3D[..3])) { tempListN3D.Add(n3D[..3]); }
            return tempListN3D;
        }

        public static List<string> FilterLetters4N3D(string name)
        {
            name = Scripts.StrRemoveSpecialLetters(name);
            name = name.ToUpper();
            name = name.Replace("-", " ");
            List<string> nameList = [];
            foreach (string _name in name.Split(' ')) { if (_name.Length > 0) { nameList.Add(_name); } }
            return nameList;
        }

        public async Task<List<User>> GetByEntry(Entry entry)
        {
            List<User> listUsers = [];
            List<Event> listEvents = await eventService.GetChildObjects(typeof(Season), entry.SeasonId);
            foreach (Event _event in listEvents)
            {
                List<User> listUsersTemp = await GetByEntryEvent(entry.Id, _event.Id);
                foreach (User user in listUsersTemp) { if (!listUsers.Contains(user)) listUsers.Add(user); }
            }
            return listUsers;
        }

        public async Task<List<User>> GetByEntryEvent(int entryId, int eventId)
        {
            List<User> listUsers = [];
            AddDto<EntryUserEvent> addDto = new();
            addDto.Dto.EntryId = entryId;
            addDto.Dto.EventId = eventId;
            List<EntryUserEvent> list = await entryUserEventService.GetByProps(addDto);
            foreach (EntryUserEvent obj in list)
            {
                User? user = await GetById(obj.UserId);
                if (user is not null) { listUsers.Add(user); }
            }
            return listUsers;
        }

        public async Task<List<User>> GetViolationsDiscordId(Season season)
        {
            List<User> list = [];
            List<Entry> listEntries = await entryService.GetChildObjects(typeof(Season), season.Id);
            foreach (Entry entry in listEntries)
            {
                List<User> userListFull = await GetByEntry(entry);
                foreach (User user in userListFull) { if (!Scripts.IsValidDiscordId(user.DiscordId) && !list.Contains(user)) { list.Add(user); } }
            }
            return list;
        }

        public async Task<List<User>> GetViolationsAllowEntriesShareDriver(Season season, bool onlyIfSameEvent = false)
        {
            List<User> list = [];
            if (!onlyIfSameEvent && season.IsAllowedEntriesShareDriver) { return list; }
            else if (onlyIfSameEvent && season.IsAllowedEntriesShareDriverSameEvent) { return list; }
            List<Event> listEvents = await eventService.GetChildObjects(typeof(Season), season.Id);
            List<EntryUserEvent> allEue = [];
            foreach (Event _event in listEvents)
            {
                List<EntryUserEvent> listEntryUserEvents = await entryUserEventService.GetChildObjects(typeof(Event), _event.Id);
                foreach (EntryUserEvent eue in listEntryUserEvents) { allEue.Add(eue); }
            }
            for (int id1 = 0; id1 < allEue.Count - 1; id1++)
            {
                for (int id2 = id1 + 1; id2 < allEue.Count; id2++)
                {
                    if (allEue[id1].EntryId != allEue[id2].EntryId && allEue[id1].UserId == allEue[id2].UserId)
                    {
                        if ((!onlyIfSameEvent || allEue[id1].EventId == allEue[id2].EventId) && !Scripts.ListContainsId(list, allEue[id1].User)) { list.Add(allEue[id1].User); }
                    }
                }
            }
            return list;
        }

        public async Task<List<User>> GetViolationsForceDriverFromOrganization(Season season)
        {
            List<User> list = [];
            if (!season.ForceDriverFromOrganization) { return list; }
            List<Entry> listEntries = await entryService.GetChildObjects(typeof(Season), season.Id);
            foreach (Entry entry in listEntries)
            {
                List<User> userListFull = await GetByEntry(entry);
                List<OrganizationUser> userListOrganization = await organizationUserService.GetChildObjects(typeof(Organization), entry.Team.OrganizationId);
                foreach (User user in userListFull)
                {
                    bool isInOrganization = false;
                    foreach (OrganizationUser organizationUser in userListOrganization) { if (organizationUser.User.Id == user.Id) { isInOrganization = true; break; } }
                    if (!isInOrganization && !list.Contains(user) && !Scripts.ListContainsId(list, user)) { list.Add(user); }
                }
            }
            return list;
        }
    }
}
