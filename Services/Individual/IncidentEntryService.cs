using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class IncidentEntryService(IIncidentEntryContext iIncidentEntryContext,
        IBaseContext<IncidentEntry> iBaseContext) : BaseService<IncidentEntry>(iBaseContext)
    {
        public bool Validate(IncidentEntry? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            //Not yet implemented

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(IncidentEntry? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            //Not yet implemented

            //Not yet implemented

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<IncidentEntry?> GetTemp() { IncidentEntry obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
