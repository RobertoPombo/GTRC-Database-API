using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class TrackService(ITrackContext iTrackContext,
        IBaseContext<Track> iBaseContext) : BaseService<Track>(iBaseContext)
    {
        public bool Validate(Track? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Track.DefaultName; isValid = false; }
            obj.AccTrackId = Scripts.RemoveSpaceStartEnd(obj.AccTrackId).Replace(" ", "_");
            if (obj.AccTrackId == string.Empty) { obj.AccTrackId = Track.DefaultAccTrackId; isValid = false; }
            obj.NameGoogleSheets = Scripts.RemoveSpaceStartEnd(obj.NameGoogleSheets);

            return isValid;
        }

        public async Task<bool> SetNextAvailable(Track? obj)
        {
            bool isAvailable = true;
            if (obj is null) { return false; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 0))
            {
                isAvailable = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            nr = 1;
            delimiter = "_";
            defName = obj.AccTrackId;
            defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 1))
            {
                isAvailable = false;
                obj.AccTrackId = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            return isAvailable;
        }

        public async Task<Track?> GetTemp() { Track obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
