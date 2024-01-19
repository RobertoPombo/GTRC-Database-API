using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeriesService(ISeriesContext iSeriesContext,
        IBaseContext<Community> iCommunityContext,
        IBaseContext<Sim> iSimContext,
        IBaseContext<Season> iSeasonContext,
        IBaseContext<Series> iBaseContext) : BaseService<Series>(iBaseContext)
    {
        public Series? Validate(Series? obj)
        {
            if (obj is null) { return null; }
            Community? community = null;
            if (obj.Community is not null) { community = iCommunityContext.GetById(obj.CommunityId).Result; };
            if (community is null)
            {
                List<Community> list = iCommunityContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Community = list[0]; obj.CommunityId = list[0].Id; }
            }
            else { obj.Community = community; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Series.DefaultName; }
            Sim? sim = null;
            if (obj.Sim is not null) { sim = iSimContext.GetById(obj.SimId).Result; };
            if (sim is null)
            {
                List<Sim> list = iSimContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Sim = list[0]; obj.SimId = list[0].Id; }
            }
            else { obj.Sim = sim; }
            if (!Scripts.IsValidDiscordId(obj.DiscordRegistrationChannelId)) { obj.DiscordRegistrationChannelId = GlobalValues.NoDiscordId; }
            if (!Scripts.IsValidDiscordId(obj.DiscordLogChannelId)) { obj.DiscordLogChannelId = GlobalValues.NoDiscordId; }
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
                nr++; if (nr == int.MaxValue)
                {
                    int startIndexCommunity = 0;
                    List<int> idListCommunity = [];
                    List<Community> listCommunity = iCommunityContext.GetAll().Result;
                    for (int index = 0; index < listCommunity.Count; index++)
                    {
                        idListCommunity.Add(listCommunity[index].Id);
                        if (listCommunity[index].Id == obj.CommunityId) { startIndexCommunity = index; }
                    }
                    int indexCommunity = startIndexCommunity;

                    if (indexCommunity < idListCommunity.Count - 1)
                    {
                        indexCommunity++;
                        obj.Community = listCommunity[indexCommunity];
                        obj.CommunityId = listCommunity[indexCommunity].Id;
                    }
                    else { indexCommunity = 0; }
                    if (indexCommunity == startIndexCommunity) { return null; }
                }
            }

            return obj;
        }

        public async Task<Series?> GetTemp() { return await SetNextAvailable(new Series()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<Season> listSeason = await iSeasonContext.GetAll();
            foreach (Season obj in listSeason) { if (obj.SeriesId == id) { return true; } }
            return false;
        }
    }
}
