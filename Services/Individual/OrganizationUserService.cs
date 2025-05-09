﻿using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class OrganizationUserService(IOrganizationUserContext iOrganizationUserContext,
        IBaseContext<Organization> iOrganizationContext,
        IBaseContext<User> iUserContext,
        IBaseContext<OrganizationUser> iBaseContext) : BaseService<OrganizationUser>(iBaseContext)
    {
        public bool Validate(OrganizationUser? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.IsInvited && obj.IsAdmin) { obj.IsAdmin = false; isValid = false; }
            List<OrganizationUser> listOrganizationUsers = GetAdmins(obj.OrganizationId);
            if (listOrganizationUsers.Count == 0 || (listOrganizationUsers.Count == 1 && listOrganizationUsers[0].UserId == obj.UserId))
            {
                if (!obj.IsAdmin || obj.IsInvited) { obj.IsInvited = false; obj.IsAdmin = true; isValid = false; }
            }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(OrganizationUser? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Organization? organization = null;
            if (obj.Organization is not null) { organization = iOrganizationContext.GetById(obj.OrganizationId).Result; };
            if (organization is null)
            {
                List<Organization> list = iOrganizationContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Organization = list[0]; obj.OrganizationId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Organization = organization; }
            User? user = null;
            if (obj.User is not null) { user = iUserContext.GetById(obj.UserId).Result; };
            if (user is null)
            {
                List<User> list = iUserContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.User = list[0]; obj.UserId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.User = user; }

            int startIndexOrganization = 0;
            List<int> idListOrganization = [];
            List<Organization> listOrganization = iOrganizationContext.GetAll().Result;
            for (int index = 0; index < listOrganization.Count; index++)
            {
                idListOrganization.Add(listOrganization[index].Id);
                if (listOrganization[index].Id == obj.OrganizationId) { startIndexOrganization = index; }
            }
            int indexOrganization = startIndexOrganization;

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
                isValidUniqProps = false;
                if (indexUser < idListUser.Count - 1) { indexUser++; }
                else { indexUser = 0; }
                obj.User = listUser[indexUser];
                obj.UserId = listUser[indexUser].Id;
                if (indexUser == startIndexUser)
                {
                    if (indexOrganization < idListOrganization.Count - 1) { indexOrganization++; }
                    else { indexOrganization = 0; }
                    obj.Organization = listOrganization[indexOrganization];
                    obj.OrganizationId = listOrganization[indexOrganization].Id;
                    if (indexOrganization == startIndexOrganization) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<OrganizationUser?> GetTemp() { OrganizationUser obj = new(); await ValidateUniqProps(obj); return obj; }

        public List<OrganizationUser> GetAdmins(int organizationId) { return iOrganizationUserContext.GetAdmins(organizationId); }

        public bool IsOnlyAdmin(OrganizationUser obj)
        {
            List<OrganizationUser> list = GetAdmins(obj.OrganizationId);
            if (list.Count == 1 && list[0].UserId == obj.UserId) { return true; }
            return false;
        }
    }
}
