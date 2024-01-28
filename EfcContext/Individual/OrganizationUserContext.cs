using Microsoft.EntityFrameworkCore;

using GTRC_Database_API.Data;
using GTRC_Database_API.Services.Interfaces;
using GTRC_Basics.Models;

namespace GTRC_Database_API.EfcContext
{
    public class OrganizationUserContext(DataContext db) : IOrganizationUserContext
    {
        public List<OrganizationUser> GetAdmins(int organizationId)
        {
            return [.. db.OrganizationsUsers.Where(obj => obj.OrganizationId == organizationId).Where(obj => obj.IsAdmin == true)];
        }
    }
}
