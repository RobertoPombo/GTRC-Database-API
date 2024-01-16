using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class ManufacturerService(IManufacturerContext iManufacturerContext, IBaseContext<Car> iCarContext, IBaseContext<Manufacturer> iBaseContext) : BaseService<Manufacturer>(iBaseContext)
    {
        public Manufacturer? Validate(Manufacturer? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Manufacturer.DefaultName; }
            return obj;
        }

        public async Task<Manufacturer?> SetNextAvailable(Manufacturer? obj)
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

        public async Task<Manufacturer?> GetTemp() { return await SetNextAvailable(new Manufacturer()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<Car> list = await iCarContext.GetAll();
            foreach (Car obj in list) { if (obj.ManufacturerId == id) { return true; } }
            return false;
        }
    }
}
