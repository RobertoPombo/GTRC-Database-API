using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.DTOs;
using GTRC_Database_API.Services.Interfaces;
using System;
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

        public async Task<List<Track>> GetByProps(TrackAddDto objDto)
        {
            List<PropertyInfo> properties = [];
            List<dynamic> values = [];
            foreach (PropertyInfo filterProperty in typeof(TrackAddDto).GetType().GetProperties())
            {
                if (filterProperty.GetValue(objDto) is not null)
                {
                    foreach (PropertyInfo property in typeof(Track).GetProperties())
                    {
                        if (filterProperty.Name == property.Name)
                        {
                            properties.Add(property);
                            values.Add(Scripts.GetCastedValue(objDto, filterProperty));
                            break;
                        }
                    }
                }
            }
            return await GetByProps(properties, values);
        }

        public async Task<List<Track>> GetByFilter(TrackFilterDtos objDto)
        {
            List<string> numericalTypes = ["System.Int16", "System.Int32", "System.Int64", "System.UInt16", "System.UInt32", "System.UInt64", "System.Single", "System.Double", "System.Decimal", "System.DateTime"];
            List<Track> list = await GetAll();
            List<Track> filteredList = [];
            foreach (Track obj in list)
            {
                bool isInList = true;
                foreach (PropertyInfo filterProperty in typeof(TrackFilterDto).GetProperties())
                {
                    var filter = filterProperty.GetValue(objDto.Filter);
                    var filterMin = filterProperty.GetValue(objDto.FilterMin);
                    var filterMax = filterProperty.GetValue(objDto.FilterMax);
                    if (filter is not null || filterMin is not null || filterMax is not null)
                    {
                        string strFilter = filter?.ToString()?.ToLower() ?? "";
                        string strFilterMin = filterMin?.ToString()?.ToLower() ?? "";
                        string strFilterMax = filterMax?.ToString()?.ToLower() ?? "";
                        foreach (PropertyInfo property in typeof(Track).GetProperties())
                        {
                            if (filterProperty.Name == property.Name)
                            {
                                var castedValue = Scripts.GetCastedValue(obj, property);
                                string strValue = castedValue.ToString().ToLower();
                                if (!strValue.Contains(strFilter)) { isInList = false; }
                                else if (numericalTypes.Contains(property.PropertyType.ToString()))
                                {
                                    if (filterMin is not null)
                                    {
                                        var castedFilterMin = Scripts.CastValue(property, filterMin);
                                        if (castedValue < castedFilterMin) { isInList = false; }
                                    }
                                    if (filterMax is not null)
                                    {
                                        var castedFilterMax = Scripts.CastValue(property, filterMax);
                                        if (castedValue > castedFilterMax) { isInList = false; }
                                    }
                                }
                                else if ((strFilterMin.Length > 0 && string.Compare(strValue, strFilterMin) == -1)
                                    || (strFilterMax.Length > 0 && string.Compare(strValue, strFilterMax) == 1))
                                {
                                    isInList = false;
                                }
                                break;
                            }
                        }
                        if (!isInList) { break; }
                    }
                }
                if (isInList) { filteredList.Add(obj); }
            }
            return filteredList;
        }

        public async Task<Track?> GetTemp() { return await SetNextAvailable(new Track()); }
    }
}
