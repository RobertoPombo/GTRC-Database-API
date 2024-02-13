using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CarclassService(ICarclassContext iCarclassContext,
        IBaseContext<Carclass> iBaseContext) : BaseService<Carclass>(iBaseContext)
    {
        public bool Validate(Carclass? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Carclass.DefaultName; isValid = false; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(Carclass? obj)
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

        public async Task<Carclass?> GetTemp() { Carclass obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
