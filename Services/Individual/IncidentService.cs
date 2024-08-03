using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class IncidentService(IIncidentContext iIncidentContext,
        IBaseContext<Incident> iBaseContext) : BaseService<Incident>(iBaseContext)
    {
        public bool Validate(Incident? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            //Not yet implemented

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Incident? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            //Not yet implemented

            //Not yet implemented

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Incident?> GetTemp() { Incident obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
