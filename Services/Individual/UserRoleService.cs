using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class UserRoleService(IUserRoleContext iUserRoleContext,
        IBaseContext<User> iUserContext,
        IBaseContext<Role> iRoleContext,
        IBaseContext<UserRole> iBaseContext) : BaseService<UserRole>(iBaseContext)
    {
        public bool Validate(UserRole? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(UserRole? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            User? user = null;
            if (obj.User is not null) { user = iUserContext.GetById(obj.UserId).Result; };
            if (user is null)
            {
                List<User> list = iUserContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.User = list[0]; obj.UserId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.User = user; }
            Role? role = null;
            if (obj.Role is not null) { role = iRoleContext.GetById(obj.RoleId).Result; };
            if (role is null)
            {
                List<Role> list = iRoleContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Role = list[0]; obj.RoleId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Role = role; }

            int startIndexRole = 0;
            List<int> idListRole = [];
            List<Role> listRole = iRoleContext.GetAll().Result;
            for (int index = 0; index < listRole.Count; index++)
            {
                idListRole.Add(listRole[index].Id);
                if (listRole[index].Id == obj.RoleId) { startIndexRole = index; }
            }
            int indexRole = startIndexRole;

            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (indexRole < idListRole.Count - 1)
                {
                    indexRole++;
                    obj.Role = listRole[indexRole];
                    obj.RoleId = listRole[indexRole].Id;
                }
                else { indexRole = 0; }
                if (indexRole == startIndexRole)
                {
                    int startIndexUser = 0;
                    List<int> idListUser = [];
                    List<User> listUser = iUserContext.GetAll().Result;
                    for (int index = 0; index < listUser.Count; index++)
                    {
                        idListUser.Add(listUser[index].Id);
                        if (listUser[index].Id == obj.UserId) { startIndexUser = index; }
                    }
                    int indexUser = startIndexUser;

                    if (indexUser < idListUser.Count - 1)
                    {
                        indexUser++;
                        obj.User = listUser[indexUser];
                        obj.UserId = listUser[indexUser].Id;
                    }
                    else { indexUser = 0; }
                    if (indexUser == startIndexUser) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<UserRole?> GetTemp() { UserRole obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
