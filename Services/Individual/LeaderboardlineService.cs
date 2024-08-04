using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class LeaderboardlineService(ILeaderboardlineContext iLeaderboardlineContext,
        IEntryContext IEntryContext,
        ISessionContext ISessionContext,
        IBaseContext<Session> iSessionContext,
        IBaseContext<Entry> iEntryContext,
        IBaseContext<User> iUserContext,
        IBaseContext<Car> iCarContext,
        UserService userService,
        IBaseContext<Leaderboardline> iBaseContext) : BaseService<Leaderboardline>(iBaseContext)
    {
        public bool Validate(Leaderboardline? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Leaderboardline? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Session? session = null;
            if (obj.Session is not null) { session = iSessionContext.GetById(obj.SessionId).Result; };
            if (session is null)
            {
                List<Session> list = iSessionContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Session = list[0]; obj.SessionId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Session = session; }
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
            Car? car = null;
            if (obj.Car is not null) { car = iCarContext.GetById(obj.CarId).Result; };
            if (car is null)
            {
                List<Car> list = iCarContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Car = list[0]; obj.CarId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Car = car; }

            int seasonId = obj.Session.Event.SeasonId;

            int startIndexSession = 0;
            List<int> idListSession = [];
            List<Session> listSession = ISessionContext.GetBySeason(seasonId);
            for (int index = 0; index < listSession.Count; index++)
            {
                idListSession.Add(listSession[index].Id);
                if (listSession[index].Id == obj.SessionId) { startIndexSession = index; }
            }
            int indexSession = startIndexSession;

            int startIndexEntry = 0;
            List<int> idListEntry = [];
            List<Entry> listEntry = IEntryContext.GetBySeason(seasonId);
            for (int index = 0; index < listEntry.Count; index++)
            {
                idListEntry.Add(listEntry[index].Id);
                if (listEntry[index].Id == obj.EntryId) { startIndexEntry = index; }
            }
            int indexEntry = startIndexEntry;

            isValidUniqProps = await ValidateUser(obj, isValidUniqProps);

            int startIndexCar = 0;
            List<int> idListCar = [];
            List<Car> listCar = iCarContext.GetAll().Result;
            for (int index = 0; index < listCar.Count; index++)
            {
                idListCar.Add(listCar[index].Id);
                if (listCar[index].Id == obj.CarId) { startIndexCar = index; }
            }
            int indexCar = startIndexCar;

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
                if (indexCar < idListCar.Count - 1) { indexCar++; }
                else { indexCar = 0; }
                obj.Car = listCar[indexCar];
                obj.CarId = listCar[indexCar].Id;
                if (indexCar == startIndexCar)
                {
                    if (indexEntry < idListEntry.Count - 1) { indexEntry++; }
                    else { indexEntry = 0; }
                    obj.Entry = listEntry[indexEntry];
                    obj.EntryId = listEntry[indexEntry].Id;
                    isValidUniqProps = await ValidateUser(obj, isValidUniqProps);
                    if (indexEntry == startIndexEntry)
                    {
                        if (indexSession < idListSession.Count - 1) { indexSession++; }
                        else { indexSession = 0; }
                        obj.Session = listSession[indexSession];
                        obj.SessionId = listSession[indexSession].Id;
                        isValidUniqProps = await ValidateUser(obj, isValidUniqProps);
                        if (indexSession == startIndexSession) { obj = null; return false; }
                    }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Leaderboardline?> GetTemp() { Leaderboardline obj = new(); await ValidateUniqProps(obj); return obj; }

        private async Task<bool> ValidateUser(Leaderboardline? obj, bool isValid)
        {
            if (obj is null) { return false; }

            List<User> listUser = await userService.GetByEntryEvent(obj.EntryId, obj.Session.EventId);
            if (listUser.Count == 0) { obj = null; return false; }
            else
            {
                bool userIsInList = false;
                foreach (User _user in listUser) { if (_user.Id == obj.UserId) { userIsInList = true; break; } }
                if (!userIsInList) { obj.User = listUser[0]; obj.UserId = listUser[0].Id; isValid = false; }
            }

            return isValid;
        }
    }
}
