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
            if (obj.IsInvited) { obj.IsAdmin = false; }
            List<OrganizationUser> listOrganizationUsers = GetAdmins(obj.Organization.Id);
            if (listOrganizationUsers.Count == 0) { obj.IsInvited = false; obj.IsAdmin = true; }
            else if (listOrganizationUsers.Count == 1 && listOrganizationUsers[0].UserId == obj.UserId) { obj.IsInvited = false; obj.IsAdmin = true; }
            return obj;
        }

        public async Task<OrganizationUser?> SetNextAvailable(OrganizationUser? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int startIndexUser = 0;
            List<int> idListUser = [];
            List<User> listUser = iUserContext.GetAll().Result;
            for (int index = 0; index < listUser.Count; index++)
            {
                idListUser.Add(listUser[index].Id);
                if (listUser[index].Id == obj.UserId) { startIndexUser = index; }
            }
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
                    int startIndexOrganization = 0;
                    List<int> idListOrganization = [];
                    List<Organization> listOrganization = iOrganizationContext.GetAll().Result;
                    for (int index = 0; index < listOrganization.Count; index++)
                    {
                        idListOrganization.Add(listOrganization[index].Id);
                        if (listOrganization[index].Id == obj.OrganizationId) { startIndexOrganization = index; }
                    }
                    int indexOrganization = startIndexOrganization;

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

        public List<OrganizationUser> GetAdmins(int organizationId) { return iOrganizationUserContext.GetAdmins(organizationId); }

        public bool IsOnlyAdmin(OrganizationUser obj)
        {
            List<OrganizationUser> list = GetAdmins(obj.OrganizationId);
            if (list.Count == 1 && list[0].UserId == obj.UserId) { return true; }
            return false;
        }
    }
}
