using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EventService(IEventContext iEventContext,
        IBaseContext<Season> iSeasonContext,
        IBaseContext<Track> iTrackContext,
        IBaseContext<EventCar> iEventCarContext,
        IBaseContext<EventCarclass> iEventCarclassContext,
        IBaseContext<Event> iBaseContext) : BaseService<Event>(iBaseContext)
    {
        public Event? Validate(Event? obj)
        {
            if (obj is null) { return null; }
            Season? season = null;
            if (obj.Season is not null) { season = iSeasonContext.GetById(obj.SeasonId).Result; };
            if (season is null)
            {
                List<Season> list = iSeasonContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Season = list[0]; obj.SeasonId = list[0].Id; }
            }
            else { obj.Season = season; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Event.DefaultName; }
            if (obj.Date > GlobalValues.DateTimeMaxValue) { obj.Date = GlobalValues.DateTimeMaxValue; }
            else if (obj.Date < GlobalValues.DateTimeMinValue) { obj.Date = GlobalValues.DateTimeMinValue; }
            Track? track = null;
            if (obj.Track is not null) { track = iTrackContext.GetById(obj.TrackId).Result; };
            if (track is null)
            {
                List<Track> list = iTrackContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Track = list[0]; obj.TrackId = list[0].Id; }
            }
            else { obj.Track = track; }
            obj.CloudLevel = Math.Min(obj.CloudLevel, Event.MaxCloudLevel);
            obj.RainLevel = Math.Min(obj.RainLevel, Event.MaxRainLevel);
            obj.WeatherRandomness = Math.Min(obj.WeatherRandomness, Event.MaxWeatherRandomness);
            return obj;
        }

        public async Task<Event?> SetNextAvailable(Event? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int startIndexSeason = 0;
            List<int> idListSeason = [];
            List<Season> listSeason = iSeasonContext.GetAll().Result;
            for (int index = 0; index < listSeason.Count; index++)
            {
                idListSeason.Add(listSeason[index].Id);
                if (listSeason[index].Id == obj.SeasonId) { startIndexSeason = index; }
            }
            int indexSeason = startIndexSeason;
            
            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 0))
            {
                obj.Name = defName + delimiter + nr.ToString();
                nr++; if (nr == int.MaxValue)
                {
                    if (indexSeason < idListSeason.Count - 1)
                    {
                        indexSeason++;
                        obj.Season = listSeason[indexSeason];
                        obj.SeasonId = listSeason[indexSeason].Id;
                    }
                    else { indexSeason = 0; }
                    if (indexSeason == startIndexSeason) { return null; }
                }
            }

            DateTime startDate = obj.Date;
            while (!await IsUnique(obj, 1))
            {
                if (obj.Date < GlobalValues.DateTimeMaxValue.AddDays(-1)) { obj.Date = obj.Date.AddDays(1); } else { obj.Date = GlobalValues.DateTimeMinValue; }
                if (obj.Date == startDate)
                {
                    if (indexSeason < idListSeason.Count - 1)
                    {
                        indexSeason++;
                        obj.Season = listSeason[indexSeason];
                        obj.SeasonId = listSeason[indexSeason].Id;
                    }
                    else { indexSeason = 0; }
                    if (indexSeason == startIndexSeason) { return null; }
                }
            }

            return obj;
        }

        public async Task<Event?> GetTemp() { return await SetNextAvailable(new Event()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<EventCar> listEventCar = await iEventCarContext.GetAll();
            foreach (EventCar obj in listEventCar) { if (obj.EventId == id) { return true; } }
            List<EventCarclass> listEventCarclass = await iEventCarclassContext.GetAll();
            foreach (EventCarclass obj in listEventCarclass) { if (obj.EventId == id) { return true; } }
            return false;
        }

        public async Task<int?> GetNr(int id)
        {
            Event? obj = await GetById(id);
            if (obj is null) { return null; }
            else
            {
                AddDto<Event> objDto = new();
                objDto.Dto.SeasonId = obj.SeasonId;
                List<Event> list = Scripts.SortByDate(await GetByProps(objDto));
                for (int index = 0; index < list.Count; index++) { if (list[index].Id == obj.Id) { return index + 1; } }
                return null;
            }
        }

        public async Task<Event?> GetNext(int seasonId, DateTime? date = null)
        {
            date ??= DateTime.UtcNow;
            Event? next = null;
            AddDto<Event> objDto = new();
            objDto.Dto.SeasonId = seasonId;
            List<Event> list = Scripts.SortByDate(await GetByProps(objDto));
            foreach (Event obj in list) { next = obj; if (obj.Date > date) { return next; } }
            return next;
        }
    }
}
