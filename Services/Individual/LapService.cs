using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class LapService(ILapContext iLapContext,
        IBaseContext<Resultsfile> iResultsfileContext,
        IBaseContext<Lap> iBaseContext) : BaseService<Lap>(iBaseContext)
    {
        public bool Validate(Lap? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.RaceNumber > Entry.MaxRaceNumber) { obj.RaceNumber = Entry.MaxRaceNumber; isValid = false; }
            else if (obj.RaceNumber < Entry.MinRaceNumber) { obj.RaceNumber = Entry.MinRaceNumber; isValid = false; }
            if (!Scripts.IsValidSteamId(obj.SteamId) && obj.SteamId != GlobalValues.NoSteamId) { obj.SteamId = GlobalValues.NoSteamId; isValid = false; }
            obj.FirstName = Scripts.RemoveSpaceStartEnd(obj.FirstName);
            if (obj.FirstName == string.Empty) { obj.FirstName = nameof(obj.FirstName); isValid = false; }
            obj.LastName = Scripts.RemoveSpaceStartEnd(obj.LastName);
            if (obj.LastName == string.Empty) { obj.LastName = nameof(obj.LastName); isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Lap? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Resultsfile? resultsfile = null;
            if (obj.Resultsfile is not null) { resultsfile = iResultsfileContext.GetById(obj.ResultsfileId).Result; };
            if (resultsfile is null)
            {
                List<Resultsfile> list = iResultsfileContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Resultsfile = list[0]; obj.ResultsfileId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Resultsfile = resultsfile; }
            if (obj.SessionLapNr < Lap.MinSessionLapNr) { obj.SessionLapNr = Lap.MinSessionLapNr; isValidUniqProps = false; }

            int startIndexResultsfile = 0;
            List<int> idListResultsfile = [];
            List<Resultsfile> listResultsfile = iResultsfileContext.GetAll().Result;
            for (int index = 0; index < listResultsfile.Count; index++)
            {
                idListResultsfile.Add(listResultsfile[index].Id);
                if (listResultsfile[index].Id == obj.ResultsfileId) { startIndexResultsfile = index; }
            }
            int indexResultsfile = startIndexResultsfile;

            uint startValue = obj.SessionLapNr;
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (obj.SessionLapNr < ushort.MaxValue) { obj.SessionLapNr += 1; } else { obj.SessionLapNr = Lap.MinSessionLapNr; }
                if (obj.SessionLapNr == startValue)
                {
                    isValidUniqProps = false;
                    if (indexResultsfile < idListResultsfile.Count - 1) { indexResultsfile++; }
                    else { indexResultsfile = 0; }
                    obj.Resultsfile = listResultsfile[indexResultsfile];
                    obj.ResultsfileId = listResultsfile[indexResultsfile].Id;
                    if (indexResultsfile == startIndexResultsfile) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Lap?> GetTemp() { Lap obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
