using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class LeaderboardlineService(ILeaderboardlineContext iLeaderboardlineContext,
        IBaseContext<Leaderboardline> iBaseContext) : BaseService<Leaderboardline>(iBaseContext)
    {
        public bool Validate(Leaderboardline? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            //Not yet implemented

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Leaderboardline? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            //Not yet implemented

            //Not yet implemented

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Leaderboardline?> GetTemp() { Leaderboardline obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
