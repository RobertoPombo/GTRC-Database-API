using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.Interfaces
{
    public interface IOrganizationUserContext
    {
        public List<OrganizationUser> GetAdmins(int organizationId);
    }
}
