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

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Series? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Sim? sim = null;
            if (obj.Sim is not null) { sim = iSimContext.GetById(obj.SimId).Result; };
            if (sim is null)
            {
                List<Sim> list = iSimContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Sim = list[0]; obj.SimId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Sim = sim; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Series.DefaultName; isValidUniqProps = false; }
            if (!Scripts.IsValidDiscordId(obj.DiscordDriverRoleId) && obj.DiscordDriverRoleId != GlobalValues.NoDiscordId)
            {
                obj.DiscordDriverRoleId = GlobalValues.NoDiscordId; isValidUniqProps = false;
            }

            int startIndexSim = 0;
            List<int> idListSim = [];
            List<Sim> listSim = iSimContext.GetAll().Result;
            for (int index = 0; index < listSim.Count; index++)
            {
                idListSim.Add(listSim[index].Id);
                if (listSim[index].Id == obj.SimId) { startIndexSim = index; }
            }
            int indexSim = startIndexSim;

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 0))
            {
                isValidUniqProps = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue)
                {
                    if (indexSim < idListSim.Count - 1) { indexSim++; }
                    else { indexSim = 0; }
                    obj.Sim = listSim[indexSim];
                    obj.SimId = listSim[indexSim].Id;
                    if (indexSim == startIndexSim) { obj = null; return false; }
                }
            }

            ulong startValue = obj.DiscordDriverRoleId;
            while (!await IsUnique(obj, 1))
            {
                isValidUniqProps = false;
                if (obj.DiscordDriverRoleId < GlobalValues.MaxDiscordId) { obj.DiscordDriverRoleId++; } else { obj.DiscordDriverRoleId = GlobalValues.MinDiscordId; }
                if (obj.DiscordDriverRoleId == startValue) { obj.DiscordDriverRoleId = GlobalValues.NoDiscordId; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Series?> GetTemp() { Series obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
