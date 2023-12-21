using GTRC_Basics;
using GTRC_Database_API.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CarService(ICarContext iCarContext, IBaseContext<Car> iBaseContext) : BaseService<Car>(iBaseContext)
    {
        private readonly int minAccCarId = 0;

        public Car Validate(Car obj)
        {
            obj.AccCarID = Math.Max(obj.AccCarID, minAccCarId);
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            obj.Manufacturer = Scripts.RemoveSpaceStartEnd(obj.Manufacturer);
            obj.Model = Scripts.RemoveSpaceStartEnd(obj.Model);
            obj.Category = Scripts.RemoveSpaceStartEnd(obj.Category);
            obj.Name_GTRC = Scripts.RemoveSpaceStartEnd(obj.Name_GTRC);
            return obj;
        }

        public Car SetNextAvailable(Car obj)
        {
            int startValue = obj.AccCarID;
            while (!IsUnique(obj))
            {
                if (obj.AccCarID < int.MaxValue) { obj.AccCarID += 1; } else { obj.AccCarID = minAccCarId; }
                if (obj.AccCarID == startValue) { break; }
            }
            return obj;
        }

        public Car GetNextAvailable() { return SetNextAvailable(new Car()); }
    }
}
