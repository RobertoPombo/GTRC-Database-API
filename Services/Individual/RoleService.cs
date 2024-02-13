using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class RoleService(IRoleContext iRoleContext,
        IBaseContext<Role> iBaseContext) : BaseService<Role>(iBaseContext)
    {
        public bool Validate(Role? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Role.DefaultName; isValid = false; }
            if (!Scripts.IsValidDiscordId(obj.DiscordId) && obj.DiscordId != GlobalValues.NoDiscordId) { obj.DiscordId = GlobalValues.NoDiscordId; isValid = false; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(Role? obj)
        {
            bool isAvailable = true;
            if (obj is null) { return false; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj))
            {
                isAvailable = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            return isAvailable;
        }

        public async Task<Role?> GetTemp() { Role obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
