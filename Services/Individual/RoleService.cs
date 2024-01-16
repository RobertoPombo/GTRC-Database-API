using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class RoleService(IRoleContext iRoleContext, IBaseContext<UserRole> iUserRoleContext, IBaseContext<Role> iBaseContext) : BaseService<Role>(iBaseContext)
    {
        public Role? Validate(Role? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Role.DefaultName; }
            return obj;
        }

        public async Task<Role?> SetNextAvailable(Role? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 0))
            {
                obj.Name = defName + delimiter + nr.ToString();
                nr++; if (nr == int.MaxValue) { return null; }
            }

            return obj;
        }

        public async Task<Role?> GetTemp() { return await SetNextAvailable(new Role()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<UserRole> listUserRole = await iUserRoleContext.GetAll();
            foreach (UserRole obj in listUserRole) { if (obj.RoleId == id) { return true; } }
            return false;
        }
    }
}
