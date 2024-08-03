using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class LapService(ILapContext iLapContext,
        IBaseContext<Lap> iBaseContext) : BaseService<Lap>(iBaseContext)
    {
        public bool Validate(Lap? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            //Not yet implemented

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Lap? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            //Not yet implemented

            //Not yet implemented

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Lap?> GetTemp() { Lap obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
