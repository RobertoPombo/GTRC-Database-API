using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SessionService(ISessionContext iSessionContext,
        IBaseContext<Session> iBaseContext) : BaseService<Session>(iBaseContext)
    {
        public bool Validate(Session? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            //Not yet implemented

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Session? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            //Not yet implemented

            //Not yet implemented

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Session?> GetTemp() { Session obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
