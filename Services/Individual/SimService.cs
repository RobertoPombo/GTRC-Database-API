using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SimService(ISimContext iSimContext,
        IBaseContext<Sim> iBaseContext) : BaseService<Sim>(iBaseContext)
    {
        public Sim? Validate(Sim? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Sim.DefaultName; }
            return obj;
        }

        public async Task<Sim?> SetNextAvailable(Sim? obj)
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
                nr++;
                if (nr == int.MaxValue) { return null; }
            }

            nr = 1;
            delimiter = " #";
            defName = obj.ShortName;
            defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 1))
            {
                obj.ShortName = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { return null; }
            }

            return obj;
        }

        public async Task<Sim?> GetTemp() { return await SetNextAvailable(new Sim()); }
    }
}
