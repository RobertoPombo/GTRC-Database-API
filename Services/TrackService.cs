using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

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

        public Track SetNextAvailable(Track obj)
        {
            int nr = 1;
            string defID = obj.AccTrackId;
            if (Scripts.SubStr(defID, -2, 1) == "_") { defID = Scripts.SubStr(defID, 0, defID.Length - 2); }
            while (!IsUnique(obj))
            {
                obj.AccTrackId = defID + "_" + nr.ToString();
                nr++; if (nr == int.MaxValue) { break; }
            }
            return obj;
        }

        public Track GetNextAvailable() { return SetNextAvailable(new Track()); }
    }
}
