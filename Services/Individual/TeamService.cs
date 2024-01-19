using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class TeamService(ITeamContext iTeamContext,
        IBaseContext<Season> iSeasonContext,
        IBaseContext<Organization> iOrganizationContext,
        IBaseContext<Team> iBaseContext) : BaseService<Team>(iBaseContext)
    {
        public Team? Validate(Team? obj)
        {
            if (obj is null) { return null; }
            Season? season = null;
            if (obj.Season is not null) { season = iSeasonContext.GetById(obj.SeasonId).Result; };
            if (season is null)
            {
                List<Season> list = iSeasonContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Season = list[0]; obj.SeasonId = list[0].Id; }
            }
            else { obj.Season = season; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Team.DefaultName; }
            Organization? organization = null;
            if (obj.Organization is not null) { organization = iOrganizationContext.GetById(obj.OrganizationId).Result; };
            if (organization is null)
            {
                List<Organization> list = iOrganizationContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Organization = list[0]; obj.OrganizationId = list[0].Id; }
            }
            else { obj.Organization = organization; }
            return obj;
        }

        public async Task<Team?> SetNextAvailable(Team? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int startIndexSeason = 0;
            List<int> idListSeason = [];
            List<Season> listSeason = iSeasonContext.GetAll().Result;
            for (int index = 0; index < listSeason.Count; index++)
            {
                idListSeason.Add(listSeason[index].Id);
                if (listSeason[index].Id == obj.SeasonId) { startIndexSeason = index; }
            }
            int indexSeason = startIndexSeason;
            
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
                    if (indexSeason < idListSeason.Count - 1)
                    {
                        indexSeason++;
                        obj.Season = listSeason[indexSeason];
                        obj.SeasonId = listSeason[indexSeason].Id;
                    }
                    else { indexSeason = 0; }
                    if (indexSeason == startIndexSeason) { return null; }
                }
            }

            return obj;
        }

        public async Task<Team?> GetTemp() { return await SetNextAvailable(new Team()); }

        public async Task<bool> HasChildObjects(int id)
        {
            return false;
        }
    }
}
