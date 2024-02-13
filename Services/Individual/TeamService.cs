using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class TeamService(ITeamContext iTeamContext,
        IBaseContext<Organization> iOrganizationContext,
        IBaseContext<Team> iBaseContext) : BaseService<Team>(iBaseContext)
    {
        public bool Validate(Team? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Team.DefaultName; isValid = false; }
            Organization? organization = null;
            if (obj.Organization is not null) { organization = iOrganizationContext.GetById(obj.OrganizationId).Result; };
            if (organization is null)
            {
                List<Organization> list = iOrganizationContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Organization = list[0]; obj.OrganizationId = list[0].Id; isValid = false; }
            }
            else { obj.Organization = organization; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(Team? obj)
        {
            bool isAvailable = true;
            if (obj is null) { return false; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj))
            {
                isAvailable = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            return isAvailable;
        }

        public async Task<Team?> GetTemp() { Team obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
