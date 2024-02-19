using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeasonService(ISeasonContext iSeasonContext,
        IBaseContext<Bop> iBopContext,
        IBaseContext<Series> iSeriesContext,
        BaseService<Event> eventService,
        IBaseContext<Season> iBaseContext) : BaseService<Season>(iBaseContext)
    {
        public bool Validate(Season? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.MinDriversPerEntry < Season.MinMinDriversPerEntry) { obj.MinDriversPerEntry = Season.MinMinDriversPerEntry; isValid = false; }
            if (obj.MaxDriversPerEntry < obj.MinDriversPerEntry) { obj.MaxDriversPerEntry = obj.MinDriversPerEntry; isValid = false; }
            if (obj.MinEntriesPerTeam < Season.MinMinEntriesPerTeam) { obj.MinEntriesPerTeam = Season.MinMinEntriesPerTeam; isValid = false; }
            if (obj.MaxEntriesPerTeam < obj.MinEntriesPerTeam) { obj.MaxEntriesPerTeam = obj.MinEntriesPerTeam; isValid = false; }
            if (obj.DateStartRegistration < GlobalValues.DateTimeMinValue) { obj.DateStartRegistration = GlobalValues.DateTimeMinValue; isValid = false; }
            if (obj.DateEndRegistration < obj.DateStartRegistration) { obj.DateEndRegistration = obj.DateStartRegistration; isValid = false; }
            if (obj.DateStartCarRegristrationLimit < GlobalValues.DateTimeMinValue) { obj.DateStartCarRegristrationLimit = GlobalValues.DateTimeMinValue; isValid = false; }
            if (obj.DateStartCarChangeLimit < GlobalValues.DateTimeMinValue) { obj.DateStartCarChangeLimit = GlobalValues.DateTimeMinValue; isValid = false; }
            Bop? bop = null;
            if (obj.Bop is not null) { bop = iBopContext.GetById(obj.BopId).Result; };
            if (bop is null)
            {
                List<Bop> list = iBopContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Bop = list[0]; obj.BopId = list[0].Id; isValid = false; }
            }
            else { obj.Bop = bop; }
            if (obj.DateBoPFreeze < GlobalValues.DateTimeMinValue) { obj.DateBoPFreeze = GlobalValues.DateTimeMinValue; isValid = false; }
            if (!Scripts.IsValidDiscordId(obj.DiscordDriverRoleId) && obj.DiscordDriverRoleId != GlobalValues.NoDiscordId)
            {
                obj.DiscordDriverRoleId = GlobalValues.NoDiscordId; isValid = false;
            }
            if (!Scripts.IsValidDiscordId(obj.DiscordRegistrationChannelId) && obj.DiscordRegistrationChannelId != GlobalValues.NoDiscordId)
            {
                obj.DiscordRegistrationChannelId = GlobalValues.NoDiscordId; isValid = false;
            }
            if (!Scripts.IsValidDiscordId(obj.DiscordTrackReportChannelId) && obj.DiscordTrackReportChannelId != GlobalValues.NoDiscordId)
            {
                obj.DiscordTrackReportChannelId = GlobalValues.NoDiscordId; isValid = false;
            }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Season? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Series? series = null;
            if (obj.Series is not null) { series = iSeriesContext.GetById(obj.SeriesId).Result; };
            if (series is null)
            {
                List<Series> list = iSeriesContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Series = list[0]; obj.SeriesId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Series = series; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Season.DefaultName; isValidUniqProps = false; }

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
                if (nr == int.MaxValue)
                {
                    int startIndexSeries = 0;
                    List<int> idListSeries = [];
                    List<Series> listSeries = iSeriesContext.GetAll().Result;
                    for (int index = 0; index < listSeries.Count; index++)
                    {
                        idListSeries.Add(listSeries[index].Id);
                        if (listSeries[index].Id == obj.SeriesId) { startIndexSeries = index; }
                    }
                    int indexSeries = startIndexSeries;

                    if (indexSeries < idListSeries.Count - 1)
                    {
                        indexSeries++;
                        obj.Series = listSeries[indexSeries];
                        obj.SeriesId = listSeries[indexSeries].Id;
                    }
                    else { indexSeries = 0; }
                    if (indexSeries == startIndexSeries) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Season?> GetTemp() { Season obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<DateTime?> GetDateFirstEvent(int seasonId, bool ofFinalEvent = false)
        {
            DateTime? date = null;
            List<Event> events = await eventService.GetChildObjects(typeof(Season), seasonId);
            foreach (Event _event in events) { if (date is null || (!ofFinalEvent && date > _event.Date) || ofFinalEvent && date < _event.Date) { date = _event.Date; } }
            return date;
        }
    }
}
