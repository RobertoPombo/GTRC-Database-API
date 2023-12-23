using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.DTOs;
using GTRC_Database_API.Services.Interfaces;
using System.Reflection;

namespace GTRC_Database_API.Services
{
    public class TrackService(ITrackContext iTrackContext, IBaseContext<Track> iBaseContext) : BaseService<Track>(iBaseContext)
    {
        private readonly int minPitBoxesCount = 0;
        private readonly int minServerSlotsCount = 0;
        private readonly int minAccTimePenDT = 0;

        public Track Validate(Track obj)
        {
            obj.AccTrackId = Scripts.RemoveSpaceStartEnd(obj.AccTrackId);
            if (obj.AccTrackId.Length == 0 ) { obj.AccTrackId = Track.DefaultAccTrackID; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            obj.PitBoxesCount = Math.Max(obj.PitBoxesCount, minPitBoxesCount);
            obj.ServerSlotsCount = Math.Max(obj.ServerSlotsCount, minServerSlotsCount);
            obj.AccTimePenDT = Math.Max(obj.AccTimePenDT, minAccTimePenDT);
            obj.NameGtrc = Scripts.RemoveSpaceStartEnd(obj.NameGtrc);
            return obj;
        }

        public async Task<Track?> SetNextAvailable(Track obj)
        {
            int nr = 1;
            string defID = obj.AccTrackId;
            if (Scripts.SubStr(defID, -2, 1) == "_") { defID = Scripts.SubStr(defID, 0, defID.Length - 2); }
            while (!await IsUnique(obj))
            {
                obj.AccTrackId = defID + "_" + nr.ToString();
                nr++; if (nr == int.MaxValue) { return null; }
            }
            return obj;
        }

        public async Task<Track?> GetByUniqProps(TrackUniqPropsDto0 objDto)
        {
            List<dynamic> listValues = [];
            for (int propertyNr = 0; propertyNr < UniqProps[0].Count; propertyNr++)
            {
                listValues.Add(Scripts.GetCastedValue(objDto.Map(), UniqProps[0][propertyNr]));
            }
            return await GetByUniqProps(listValues);
        }

        public async Task<List<Track>> GetBy(TrackAddDto objDto)
        {
            List<PropertyInfo> properties = [];
            List<dynamic> values = [];
            foreach (PropertyInfo property in objDto.GetType().GetProperties())
            {
                if (property.GetValue(objDto) is not null)
                {
                    foreach (PropertyInfo baseProperty in typeof(Track).GetProperties())
                    {
                        if (property.Name == baseProperty.Name)
                        {
                            properties.Add(baseProperty);
                            values.Add(Scripts.GetCastedValue(objDto, property));
                            break;
                        }
                    }
                }
            }
            return await GetBy(properties, values);
        }

        public async Task<List<Track>> GetByFilter(TrackFilterDto objDto, TrackFilterDto objDtoMin, TrackFilterDto objDtoMax)
        {
            List<Track> list = await GetAll();
            foreach (Track obj in list)
            {

            }
            return list;
        }

        public async Task<Track?> GetTemp() { return await SetNextAvailable(new Track()); }
    }
}
