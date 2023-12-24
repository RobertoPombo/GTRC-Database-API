using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeriesService(ISeriesContext iSeriesContext, IBaseContext<Series> iBaseContext) : BaseService<Series>(iBaseContext)
    {
        public static Series Validate(Series obj)
        {
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name.Length == 0) { obj.Name = Series.DefaultName; }
            return obj;
        }

        public async Task<Series?> SetNextAvailable(Series obj)
        {
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

        public async Task<Series?> GetTemp() { return await SetNextAvailable(Validate(new Series())); }
    }
}
