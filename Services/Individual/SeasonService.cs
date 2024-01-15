using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.EfcContext;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeasonService(ISeasonContext iSeasonContext, IBaseContext<Series> iSeriesContext, IBaseContext<Season> iBaseContext) : BaseService<Season>(iBaseContext)
    {
        public Season? Validate(Season? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Season.DefaultName; }
            Series? series = null;
            if (obj.Series is not null) { series = iSeriesContext.GetById(obj.SeriesId).Result; };
            if (series is null)
            {
                List<Series> list = iSeriesContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Series = list[0]; obj.SeriesId = list[0].Id; }
            }
            else { obj.Series = series; }
            if (obj.MinDriversPerEntry < Season.MinMinDriversPerEntry) { obj.MinDriversPerEntry = Season.MinMinDriversPerEntry; }
            if (obj.MinDriversPerEntry > obj.MaxDriversPerEntry) { obj.MinDriversPerEntry = obj.MaxDriversPerEntry; }
            if (obj.DateRegisterLimit < GlobalValues.DateTimeMinValue) { obj.DateRegisterLimit = GlobalValues.DateTimeMinValue; }
            if (obj.DateBoPFreeze < GlobalValues.DateTimeMinValue) { obj.DateBoPFreeze = GlobalValues.DateTimeMinValue; }
            if (obj.DateCarChangeLimit < GlobalValues.DateTimeMinValue) { obj.DateCarChangeLimit = GlobalValues.DateTimeMinValue; }
            if (!Scripts.IsValidDiscordId(obj.DiscordRoleId)) { obj.DiscordRoleId = GlobalValues.NoDiscordId; }
            if (!Scripts.IsValidDiscordId(obj.DiscordChannelId)) { obj.DiscordChannelId = GlobalValues.NoDiscordId; }
            return obj;
        }

        public async Task<Season?> SetNextAvailable(Season? obj)
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

        public async Task<Season?> GetTemp() { return await SetNextAvailable(new Season()); }
    }
}
