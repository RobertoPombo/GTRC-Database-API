using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeasonService(ISeasonContext iSeasonContext,
        IBaseContext<Bop> iBopContext,
        IBaseContext<Series> iSeriesContext,
        IBaseContext<Season> iBaseContext) : BaseService<Season>(iBaseContext)
    {
        public bool Validate(Season? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.MinDriversPerEntryEvent < Season.MinMinDriversPerEntryEvent) { obj.MinDriversPerEntryEvent = Season.MinMinDriversPerEntryEvent; isValid = false; }
            if (obj.MaxDriversPerEntryEvent < obj.MinDriversPerEntryEvent) { obj.MaxDriversPerEntryEvent = obj.MinDriversPerEntryEvent; isValid = false; }
            if (obj.MinEntriesPerTeam < Season.MinMinEntriesPerTeam) { obj.MinEntriesPerTeam = Season.MinMinEntriesPerTeam; isValid = false; }
            if (obj.MaxEntriesPerTeam < obj.MinEntriesPerTeam) { obj.MaxEntriesPerTeam = obj.MinEntriesPerTeam; isValid = false; }
            if (obj.DateStartRegistration < GlobalValues.DateTimeMinValue) { obj.DateStartRegistration = GlobalValues.DateTimeMinValue; isValid = false; }
            else if(obj.DateStartRegistration > GlobalValues.DateTimeMaxValue) { obj.DateStartRegistration = GlobalValues.DateTimeMaxValue; isValid = false; }
            if (obj.DateEndRegistration < obj.DateStartRegistration) { obj.DateEndRegistration = obj.DateStartRegistration; isValid = false; }
            else if(obj.DateEndRegistration > GlobalValues.DateTimeMaxValue) { obj.DateEndRegistration = GlobalValues.DateTimeMaxValue; isValid = false; }
            if (obj.DateStartCarRegistrationLimit < GlobalValues.DateTimeMinValue) { obj.DateStartCarRegistrationLimit = GlobalValues.DateTimeMinValue; isValid = false; }
            else if(obj.DateStartCarRegistrationLimit > GlobalValues.DateTimeMaxValue) { obj.DateStartCarRegistrationLimit = GlobalValues.DateTimeMaxValue; isValid = false; }
            if (obj.DateStartCarChangeLimit < GlobalValues.DateTimeMinValue) { obj.DateStartCarChangeLimit = GlobalValues.DateTimeMinValue; isValid = false; }
            else if(obj.DateStartCarChangeLimit > GlobalValues.DateTimeMaxValue) { obj.DateStartCarChangeLimit = GlobalValues.DateTimeMaxValue; isValid = false; }
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
            else if (obj.DateBoPFreeze > GlobalValues.DateTimeMaxValue) { obj.DateBoPFreeze = GlobalValues.DateTimeMaxValue; isValid = false; }
            if (obj.DateEndAutoGenerateRaceNumbers < GlobalValues.DateTimeMinValue) { obj.DateEndAutoGenerateRaceNumbers = GlobalValues.DateTimeMinValue; isValid = false; }
            else if (obj.DateEndAutoGenerateRaceNumbers > GlobalValues.DateTimeMaxValue) { obj.DateEndAutoGenerateRaceNumbers = GlobalValues.DateTimeMaxValue; isValid = false; }
            if (obj.DateEndChangeTeam < GlobalValues.DateTimeMinValue) { obj.DateEndChangeTeam = GlobalValues.DateTimeMinValue; isValid = false; }
            else if (obj.DateEndChangeTeam > GlobalValues.DateTimeMaxValue) { obj.DateEndChangeTeam = GlobalValues.DateTimeMaxValue; isValid = false; }

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

            int startIndexSeries = 0;
            List<int> idListSeries = [];
            List<Series> listSeries = iSeriesContext.GetAll().Result;
            for (int index = 0; index < listSeries.Count; index++)
            {
                idListSeries.Add(listSeries[index].Id);
                if (listSeries[index].Id == obj.SeriesId) { startIndexSeries = index; }
            }
            int indexSeries = startIndexSeries;

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
                    if (indexSeries < idListSeries.Count - 1) { indexSeries++; }
                    else { indexSeries = 0; }
                    obj.Series = listSeries[indexSeries];
                    obj.SeriesId = listSeries[indexSeries].Id;
                    if (indexSeries == startIndexSeries) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Season?> GetTemp() { Season obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<Season?> GetCurrent(int seriesId, DateTime date)
        {
            Season? next = null;
            List<Season> list = Scripts.SortByDate(await GetChildObjects(typeof(Series), seriesId));
            foreach (Season obj in list) { if (obj.DateStartRegistration > date) { return next; } next = obj; }
            return next;
        }
    }
}
