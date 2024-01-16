using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.EfcContext;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeriesService(ISeriesContext iSeriesContext, IBaseContext<Season> iSeasonContext, IBaseContext<Series> iBaseContext) : BaseService<Series>(iBaseContext)
    {
        public Series? Validate(Series? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Series.DefaultName; }
            return obj;
        }

        public async Task<Series?> SetNextAvailable(Series? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj))
            {
                obj.Name = defName + delimiter + nr.ToString();
                nr++; if (nr == int.MaxValue) { return null; }
            }

            return obj;
        }

        public async Task<Series?> GetTemp() { return await SetNextAvailable(new Series()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<Season> list = await iSeasonContext.GetAll();
            foreach (Season obj in list) { if (obj.SeriesId == id) { return true; } }
            return false;
        }
    }
}
