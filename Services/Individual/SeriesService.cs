using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeriesService(ISeriesContext iSeriesContext,
        IBaseContext<Sim> iSimContext,
        IBaseContext<Series> iBaseContext) : BaseService<Series>(iBaseContext)
    {
        public bool Validate(Series? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Sim? sim = null;
            if (obj.Sim is not null) { sim = iSimContext.GetById(obj.SimId).Result; };
            if (sim is null)
            {
                List<Sim> list = iSimContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Sim = list[0]; obj.SimId = list[0].Id; isValid = false; }
            }
            else { obj.Sim = sim; }
            if (!Scripts.IsValidDiscordId(obj.DiscordRegistrationChannelId) && obj.DiscordRegistrationChannelId != GlobalValues.NoDiscordId)
            {
                obj.DiscordRegistrationChannelId = GlobalValues.NoDiscordId; isValid = false;
            }
            if (!Scripts.IsValidDiscordId(obj.DiscordLogChannelId) && obj.DiscordLogChannelId != GlobalValues.NoDiscordId)
            {
                obj.DiscordLogChannelId = GlobalValues.NoDiscordId; isValid = false;
            }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Series? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }
            
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Series.DefaultName; isValidUniqProps = false; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Series?> GetTemp() { Series obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
