using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class OrganizationService(IOrganizationContext iOrganizationContext,
        IBaseContext<Organization> iBaseContext) : BaseService<Organization>(iBaseContext)
    {
        public bool Validate(Organization? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.RegisterDate > DateTime.Now || obj.RegisterDate < GlobalValues.DateTimeMinValue) { obj.RegisterDate = DateTime.Now; isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Organization? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Organization.DefaultName; isValidUniqProps = false; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Organization?> GetTemp() { Organization obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
