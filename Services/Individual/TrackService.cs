using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class TrackService(ITrackContext iTrackContext,
        IBaseContext<BopTrackCar> iBopTrackCarContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Track> iBaseContext) : BaseService<Track>(iBaseContext)
    {
        public Track? Validate(Track? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Track.DefaultName; }
            obj.AccTrackId = Scripts.RemoveSpaceStartEnd(obj.AccTrackId).Replace(" ", "_");
            if (obj.AccTrackId.Length == 0) { obj.AccTrackId = Track.DefaultAccTrackId; }
            obj.NameGoogleSheets = Scripts.RemoveSpaceStartEnd(obj.NameGoogleSheets);
            return obj;
        }

        public async Task<Track?> SetNextAvailable(Track? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 0))
            {
                obj.Name = defName + delimiter + nr.ToString();
                nr++; if (nr == int.MaxValue) { return null; }
            }

            nr = 1;
            delimiter = "_";
            defName = obj.AccTrackId;
            defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 1))
            {
                obj.AccTrackId = defName + delimiter + nr.ToString();
                nr++; if (nr == int.MaxValue) { return null; }
            }

            return obj;
        }

        public async Task<Track?> GetTemp() { return await SetNextAvailable(new Track()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<BopTrackCar> listBopTrackCar = await iBopTrackCarContext.GetAll();
            foreach (BopTrackCar obj in listBopTrackCar) { if (obj.TrackId == id) { return true; } }
            List<Event> listEvent = await iEventContext.GetAll();
            foreach (Event obj in listEvent) { if (obj.TrackId == id) { return true; } }
            return false;
        }
    }
}
