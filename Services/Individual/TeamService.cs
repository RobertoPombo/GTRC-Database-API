using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class TeamService(ITeamContext iTeamContext,
        BaseService<Entry> entryService,
        IBaseContext<Organization> iOrganizationContext,
        IBaseContext<Team> iBaseContext) : BaseService<Team>(iBaseContext)
    {
        public bool Validate(Team? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

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

        public async Task<bool> ValidateUniqProps(Team? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Team.DefaultName; isValidUniqProps = false; }

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

        public async Task<Team?> GetTemp() { Team obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<List<Team>> GetBySeason(Season season)
        {
            List<Team> list = [];
            List<Entry> listEntries = await entryService.GetChildObjects(typeof(Season), season.Id);
            foreach (Entry entry in listEntries) { if (!Scripts.ListContainsId(list, entry.Team)) { list.Add(entry.Team); } }
            return list;
        }

        public async Task<List<Team>> GetViolationsMinEntriesPerTeam(Season season, bool isGetViolationsMaxEntriesPerTeam = false)
        {
            List<Team> list = [];
            List<Team> listAll = await GetAll();
            foreach (Team team in listAll)
            {
                AddDto<Entry> addDtoEntry = new() { Dto = new EntryAddDto() { SeasonId = season.Id, TeamId = team.Id } };
                List<Entry> listEntries = await entryService.GetByProps(addDtoEntry);
                if (listEntries.Count > 0)
                {
                    if (!isGetViolationsMaxEntriesPerTeam && listEntries.Count < season.MinEntriesPerTeam) { list.Add(team); }
                    else if (isGetViolationsMaxEntriesPerTeam && listEntries.Count > season.MaxEntriesPerTeam) { list.Add(team); }
                }
            }
            return list;
        }
    }
}
