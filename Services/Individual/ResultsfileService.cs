using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class ResultsfileService(IResultsfileContext iResultsfileContext,
        IBaseContext<Resultsfile> iBaseContext) : BaseService<Resultsfile>(iBaseContext)
    {
        public bool Validate(Resultsfile? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            //Not yet implemented

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Resultsfile? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            //Not yet implemented

            //Not yet implemented

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Resultsfile?> GetTemp() { Resultsfile obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
