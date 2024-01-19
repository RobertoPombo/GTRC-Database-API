using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CommunityService(ICommunityContext iCommunityContext,
        IBaseContext<User> iUserContext,
        IBaseContext<Series> iSeriesContext,
        IBaseContext<Community> iBaseContext) : BaseService<Community>(iBaseContext)
    {
        public Community? Validate(Community? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Community.DefaultName; }
            return obj;
        }

        public async Task<Community?> SetNextAvailable(Community? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj))
            {
                obj.Name = defName + delimiter + nr.ToString();
                nr++; if (nr == int.MaxValue) { return null; }
            }

            return obj;
        }

        public async Task<Community?> GetTemp() { return await SetNextAvailable(new Community()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<User> listUser = await iUserContext.GetAll();
            foreach (User obj in listUser) { if (obj.CommunityId == id) { return true; } }
            List<Series> listSeries = await iSeriesContext.GetAll();
            foreach (Series obj in listSeries) { if (obj.CommunityId == id) { return true; } }
            return false;
        }
    }
}
