using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class BopService(IBopContext iBopContext,
        IBaseContext<BopTrackCar> iBopTrackCarContext,
        IBaseContext<Bop> iBaseContext) : BaseService<Bop>(iBaseContext)
    {
        public Bop? Validate(Bop? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Bop.DefaultName; }
            return obj;
        }

        public async Task<Bop?> SetNextAvailable(Bop? obj)
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

        public async Task<Bop?> GetTemp() { return await SetNextAvailable(new Bop()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<BopTrackCar> listBopTrackCar = await iBopTrackCarContext.GetAll();
            foreach (BopTrackCar obj in listBopTrackCar) { if (obj.BopId == id) { return true; } }
            return false;
        }
    }
}
