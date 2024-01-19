using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CarService(ICarContext iCarContext,
        IBaseContext<Carclass> iCarclassContext,
        IBaseContext<Manufacturer> iManufacturerContext,
        IBaseContext<BopTrackCar> iBopTrackCarContext,
        IBaseContext<EventCar> iEventCarContext,
        IBaseContext<Car> iBaseContext) : BaseService<Car>(iBaseContext)
    {
        public Car? Validate(Car? obj)
        {
            if (obj is null) { return null; }
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Car.DefaultName; }
            Manufacturer? manufacturer = null;
            if (obj.Manufacturer is not null) { manufacturer = iManufacturerContext.GetById(obj.ManufacturerId).Result; };
            if (manufacturer is null)
            {
                List<Manufacturer> list = iManufacturerContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Manufacturer = list[0]; obj.ManufacturerId = list[0].Id; }
            }
            else { obj.Manufacturer = manufacturer; }
            obj.Model = Scripts.RemoveSpaceStartEnd(obj.Model);
            Carclass? carclass = null;
            if (obj.Carclass is not null) { carclass = iCarclassContext.GetById(obj.CarclassId).Result; };
            if (carclass is null)
            {
                List<Carclass> list = iCarclassContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Carclass = list[0]; obj.CarclassId = list[0].Id; }
            }
            else { obj.Carclass = carclass; }
            obj.WidthMm = Math.Max(obj.WidthMm, (ushort)1);
            obj.LengthMm = Math.Max(obj.LengthMm, (ushort)1);
            obj.NameGtrc = Scripts.RemoveSpaceStartEnd(obj.NameGtrc);
            return obj;
        }

        public async Task<Car?> SetNextAvailable(Car? obj)
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

            uint startValue = obj.AccCarId;
            while (!await IsUnique(obj, 1))
            {
                if (obj.AccCarId < uint.MaxValue) { obj.AccCarId += 1; } else { obj.AccCarId = uint.MinValue; }
                if (obj.AccCarId == startValue) { return null; }
            }

            return obj;
        }

        public async Task<Car?> GetTemp() { return await SetNextAvailable(new Car()); }

        public async Task<bool> HasChildObjects(int id)
        {
            List<BopTrackCar> listBopTrackCar = await iBopTrackCarContext.GetAll();
            foreach (BopTrackCar obj in listBopTrackCar) { if (obj.CarId == id) { return true; } }
            List<EventCar> listEventCar = await iEventCarContext.GetAll();
            foreach (EventCar obj in listEventCar) { if (obj.EventId == id) { return true; } }
            return false;
        }
    }
}
