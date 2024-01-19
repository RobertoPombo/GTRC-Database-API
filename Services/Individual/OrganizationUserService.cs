using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class OrganizationUserService(IOrganizationUserContext iOrganizationUserContext,
        IBaseContext<Organization> iOrganizationContext,
        IBaseContext<User> iUserContext,
        IBaseContext<OrganizationUser> iBaseContext) : BaseService<OrganizationUser>(iBaseContext)
    {
        public OrganizationUser? Validate(OrganizationUser? obj)
        {
            if (obj is null) { return null; }
            Organization? organization = null;
            if (obj.Organization is not null) { organization = iOrganizationContext.GetById(obj.OrganizationId).Result; };
            if (organization is null)
            {
                List<Organization> list = iOrganizationContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Organization = list[0]; obj.OrganizationId = list[0].Id; }
            }
            else { obj.Organization = organization; }
            User? user = null;
            if (obj.User is not null) { user = iUserContext.GetById(obj.UserId).Result; };
            if (user is null)
            {
                List<User> list = iUserContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.User = list[0]; obj.UserId = list[0].Id; }
            }
            else { obj.User = user; }
            return obj;
        }

        public async Task<OrganizationUser?> SetNextAvailable(OrganizationUser? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int startIndexOrganization = 0;
            int startIndexUser = 0;
            List<int> idListOrganization = [];
            List<int> idListUser = [];
            List<Organization> listOrganization = iOrganizationContext.GetAll().Result;
            List<User> listUser = iUserContext.GetAll().Result;
            for (int index = 0; index < listOrganization.Count; index++)
            {
                idListOrganization.Add(listOrganization[index].Id);
                if (listOrganization[index].Id == obj.OrganizationId) { startIndexOrganization = index; }
            }
            for (int index = 0; index < listUser.Count; index++)
            {
                idListUser.Add(listUser[index].Id);
                if (listUser[index].Id == obj.UserId) { startIndexUser = index; }
            }
            int indexOrganization = startIndexOrganization;
            int indexUser = startIndexUser;

            while (!await IsUnique(obj))
            {
                if (indexUser < idListUser.Count - 1)
                {
                    indexUser++;
                    obj.User = listUser[indexUser];
                    obj.UserId = listUser[indexUser].Id;
                }
                else { indexUser = 0; }
                if (indexUser == startIndexUser)
                {
                    if (indexOrganization < idListOrganization.Count - 1)
                    {
                        indexOrganization++;
                        obj.Organization = listOrganization[indexOrganization];
                        obj.OrganizationId = listOrganization[indexOrganization].Id;
                    }
                    else { indexOrganization = 0; }
                    if (indexOrganization == startIndexOrganization) { return null; }
                }
            }

            return obj;
        }

        public async Task<OrganizationUser?> GetTemp() { return await SetNextAvailable(new OrganizationUser()); }

        public async Task<bool> HasChildObjects(int id)
        {
            return false;
        }
    }
}
