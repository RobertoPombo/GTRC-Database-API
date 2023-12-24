using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class TrackService(ITrackContext iTrackContext, IBaseContext<Track> iBaseContext) : BaseService<Track>(iBaseContext)
    {
        public static Track Validate(Track obj)
        {
            obj.AccTrackId = Scripts.RemoveSpaceStartEnd(obj.AccTrackId).Replace(" ", "_");
            if (obj.AccTrackId.Length == 0 ) { obj.AccTrackId = Track.DefaultAccTrackId; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            obj.NameGtrc = Scripts.RemoveSpaceStartEnd(obj.NameGtrc);
            return obj;
        }

        public async Task<Track?> SetNextAvailable(Track obj)
        {
            int nr = 1;
            string delimiter = "_";
            string defName = obj.AccTrackId;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length - delimiter.Length)]; }
            while (!await IsUnique(obj))
            {
                obj.AccTrackId = defName + delimiter + nr.ToString();
                nr++; if (nr == int.MaxValue) { return null; }
            }
            return obj;
        }

        public async Task<Track?> GetTemp() { return await SetNextAvailable(Validate(new Track())); }
    }
}
