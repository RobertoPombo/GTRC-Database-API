using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeasonService(ISeasonContext iSeasonContext,
        IBaseContext<Bop> iBopContext,
        IBaseContext<Series> iSeriesContext,
        IBaseContext<Team> iTeamContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Season> iBaseContext) : BaseService<Season>(iBaseContext)
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
            if (obj.MaxDriversPerEntry < obj.MinDriversPerEntry) { obj.MaxDriversPerEntry = obj.MinDriversPerEntry; }
            if (obj.MinEntriesPerTeam < Season.MinMinEntriesPerTeam) { obj.MinEntriesPerTeam = Season.MinMinEntriesPerTeam; }
            if (obj.MaxEntriesPerTeam < obj.MinEntriesPerTeam) { obj.MaxEntriesPerTeam = obj.MinEntriesPerTeam; }
            if (obj.DateStartRegistration < GlobalValues.DateTimeMinValue) { obj.DateStartRegistration = GlobalValues.DateTimeMinValue; }
            if (obj.DateEndRegistration < obj.DateStartRegistration) { obj.DateEndRegistration = obj.DateStartRegistration; }
            if (obj.DateStartCarRegristrationLimit < GlobalValues.DateTimeMinValue) { obj.DateStartCarRegristrationLimit = GlobalValues.DateTimeMinValue; }
            if (obj.DateStartCarChangeLimit < GlobalValues.DateTimeMinValue) { obj.DateStartCarChangeLimit = GlobalValues.DateTimeMinValue; }
            Bop? bop = null;
            if (obj.Bop is not null) { bop = iBopContext.GetById(obj.BopId).Result; };
            if (bop is null)
            {
                List<Bop> list = iBopContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Bop = list[0]; obj.BopId = list[0].Id; }
            }
            else { obj.Bop = bop; }
            if (obj.DateBoPFreeze < GlobalValues.DateTimeMinValue) { obj.DateBoPFreeze = GlobalValues.DateTimeMinValue; }
            if (!Scripts.IsValidDiscordId(obj.DiscordDriverRoleId)) { obj.DiscordDriverRoleId = GlobalValues.NoDiscordId; }
            if (!Scripts.IsValidDiscordId(obj.DiscordRegistrationChannelId)) { obj.DiscordRegistrationChannelId = GlobalValues.NoDiscordId; }
            if (!Scripts.IsValidDiscordId(obj.DiscordTrackReportChannelId)) { obj.DiscordTrackReportChannelId = GlobalValues.NoDiscordId; }
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

        public async Task<bool> HasChildObjects(int id)
        {
            List<Team> listTeam = await iTeamContext.GetAll();
            foreach (Team obj in listTeam) { if (obj.OrganizationId == id) { return true; } }
            List<Event> listEvents = await iEventContext.GetAll();
            foreach (Event obj in listEvents) { if (obj.SeasonId == id) { return true; } }
            return false;
        }
    }
}
