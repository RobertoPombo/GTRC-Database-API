using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EntryService(IEntryContext iEntryContext,
        IBaseContext<Car> iCarContext,
        IBaseContext<Season> iSeasonContext,
        IBaseContext<Team> iTeamContext,
        IBaseContext<Entry> iBaseContext) : BaseService<Entry>(iBaseContext)
    {
        public bool Validate(Entry? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Season? season = null;
            if (obj.Season is not null) { season = iSeasonContext.GetById(obj.SeasonId).Result; };
            if (season is null)
            {
                List<Season> list = iSeasonContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Season = list[0]; obj.SeasonId = list[0].Id; isValid = false; }
            }
            else { obj.Season = season; }
            if (obj.RaceNumber > Entry.MaxRaceNumber) { obj.RaceNumber = Entry.MaxRaceNumber; isValid = false; }
            else if (obj.RaceNumber < Entry.MinRaceNumber) { obj.RaceNumber = Entry.MinRaceNumber; isValid = false; }
            Team? team = null;
            if (obj.Team is not null) { team = iTeamContext.GetById(obj.TeamId).Result; };
            if (team is null)
            {
                List<Team> list = iTeamContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Team = list[0]; obj.TeamId = list[0].Id; isValid = false; }
            }
            else { obj.Team = team; }
            Car? car = null;
            if (obj.Car is not null) { car = iCarContext.GetById(obj.CarId).Result; };
            if (car is null)
            {
                List<Car> list = iCarContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Car = list[0]; obj.CarId = list[0].Id; isValid = false; }
            }
            else { obj.Car = car; }
            if (obj.RegisterDate > DateTime.UtcNow || obj.RegisterDate < GlobalValues.DateTimeMinValue) { obj.RegisterDate = DateTime.UtcNow; isValid = false; }
            if (obj.SignOutDate > GlobalValues.DateTimeMaxValue || obj.SignOutDate < obj.RegisterDate) { obj.SignOutDate = GlobalValues.DateTimeMaxValue; isValid = false; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(Entry? obj)
        {
            bool isAvailable = true;
            if (obj is null) { return false; }

            int startRaceNumber = obj.RaceNumber;
            while (!await IsUnique(obj))
            {
                isAvailable = false;
                if (obj.RaceNumber < Entry.MaxRaceNumber) { obj.RaceNumber += 1; } else { obj.RaceNumber = Entry.DefaultRaceNumber; }
                if (obj.RaceNumber == startRaceNumber)
                {
                    int startIndexSeason = 0;
                    List<int> idListSeason = [];
                    List<Season> listSeason = iSeasonContext.GetAll().Result;
                    for (int index = 0; index < listSeason.Count; index++)
                    {
                        idListSeason.Add(listSeason[index].Id);
                        if (listSeason[index].Id == obj.SeasonId) { startIndexSeason = index; }
                    }
                    int indexSeason = startIndexSeason;

                    if (indexSeason < idListSeason.Count - 1)
                    {
                        indexSeason++;
                        obj.Season = listSeason[indexSeason];
                        obj.SeasonId = listSeason[indexSeason].Id;
                    }
                    else { indexSeason = 0; }
                    if (indexSeason == startIndexSeason) { obj = null; return false; }
                }
            }

            return isAvailable;
        }

        public async Task<Entry?> GetTemp() { Entry obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
