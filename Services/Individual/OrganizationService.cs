using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class OrganizationService(IOrganizationContext iOrganizationContext,
        IBaseContext<OrganizationUser> iOrganizationUserContext,
        IBaseContext<Team> iTeamContext,
        IBaseContext<Organization> iBaseContext) : BaseService<Organization>(iBaseContext)
    {
        public Organization? Validate(Organization? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Organization.DefaultName; }
            return obj;
        }

        public async Task<Organization?> SetNextAvailable(Organization? obj)
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
                nr++;
                if (nr == int.MaxValue) { return null; }
            }

            return obj;
        }

        public async Task<Organization?> GetTemp() { return await SetNextAvailable(new Organization()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<OrganizationUser> listOrganizationUser = await iOrganizationUserContext.GetAll();
            foreach (OrganizationUser obj in listOrganizationUser) { if (obj.OrganizationId == id) { return true; } }
            List<Team> listTeam = await iTeamContext.GetAll();
            foreach (Team obj in listTeam) { if (obj.OrganizationId == id) { return true; } }
            return false;
        }
    }
}
