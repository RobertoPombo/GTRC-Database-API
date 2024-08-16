using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;
using GTRC_Database_API.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace GTRC_Database_API.Services
{
    public class EventService(IEventContext iEventContext,
        IBaseContext<Season> iSeasonContext,
        IBaseContext<Track> iTrackContext,
        IBaseContext<Event> iBaseContext) : BaseService<Event>(iBaseContext)
    {
        public bool Validate(Event? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Track? track = null;
            if (obj.Track is not null) { track = iTrackContext.GetById(obj.TrackId).Result; };
            if (track is null)
            {
                List<Track> list = iTrackContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Track = list[0]; obj.TrackId = list[0].Id; isValid = false; }
            }
            else { obj.Track = track; }
            if (obj.CloudLevel > Event.MaxCloudLevel) { obj.CloudLevel = Event.MaxCloudLevel; isValid = false; }
            if (obj.RainLevel > Event.MaxRainLevel) { obj.RainLevel = Event.MaxRainLevel; isValid = false; }
            if (obj.WeatherRandomness > Event.MaxWeatherRandomness) { obj.WeatherRandomness = Event.MaxWeatherRandomness; isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Event? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Season? season = null;
            if (obj.Season is not null) { season = iSeasonContext.GetById(obj.SeasonId).Result; };
            if (season is null)
            {
                List<Season> list = iSeasonContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Season = list[0]; obj.SeasonId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Season = season; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Event.DefaultName; isValidUniqProps = false; }
            if (obj.Date > GlobalValues.DateTimeMaxValue) { obj.Date = GlobalValues.DateTimeMaxValue; isValidUniqProps = false; }
            else if (obj.Date < GlobalValues.DateTimeMinValue) { obj.Date = GlobalValues.DateTimeMinValue; isValidUniqProps = false; }

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
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 0))
            {
                isValidUniqProps = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue)
                {
                    if (indexSeason < idListSeason.Count - 1) { indexSeason++; }
                    else { indexSeason = 0; }
                    obj.Season = listSeason[indexSeason];
                    obj.SeasonId = listSeason[indexSeason].Id;
                    if (indexSeason == startIndexSeason) { obj = null; return false; }
                }
            }

            DateTime startDate = obj.Date;
            while (!await IsUnique(obj, 1))
            {
                isValidUniqProps = false;
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
                    if (indexSeason == startIndexSeason) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Event?> GetTemp() { Event obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<int?> GetNr(int id)
        {
            Event? obj = await GetById(id);
            if (obj is null) { return null; }
            else
            {
                int eventNr = 0;
                if (obj.IsPreQualifying) { return eventNr; }
                AddDto<Event> objDto = new();
                objDto.Dto.SeasonId = obj.SeasonId;
                List<Event> list = Scripts.SortByDate(await GetByProps(objDto));
                foreach (Event _obj in list)
                {
                    if (!_obj.IsPreQualifying) { eventNr++; }
                    if (_obj.Id == obj.Id) { return eventNr; }
                }
                return null;
            }
        }

        public async Task<Event?> GetByNr(int seasonId, int nr)
        {
            List<Event> list = Scripts.SortByDate(await GetChildObjects(typeof(Season), seasonId));
            if (nr < 1 || nr > list.Count) { return null; }
            return list[nr - 1];
        }

        public async Task<Event?> GetNext(int seasonId, DateTime date)
        {
            Event? next = null;
            List<Event> list = Scripts.SortByDate(await GetChildObjects(typeof(Season), seasonId));
            foreach (Event obj in list) { next = obj; if (obj.Date > date) { return next; } }
            return next;
        }

        public async Task<Event?> GetFirst(int seasonId, bool isGetFinal = false)
        {
            Event? _event = null;
            List<Event> events = await GetChildObjects(typeof(Season), seasonId);
            foreach (Event _eventCandidate in events)
            {
                if (_event is null || (!isGetFinal && _event.Date > _eventCandidate.Date) || (isGetFinal && _event.Date < _eventCandidate.Date)) { _event = _eventCandidate; }
            }
            return _event;
        }
    }
}
