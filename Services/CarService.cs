using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CarService(ICarContext iCarContext, IBaseContext<Car> iBaseContext) : BaseService<Car>(iBaseContext)
    {
        private readonly int minAccCarId = 0;

        public Car Validate(Car obj)
        {
            obj.AccCarId = Math.Max(obj.AccCarId, minAccCarId);
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            obj.Manufacturer = Scripts.RemoveSpaceStartEnd(obj.Manufacturer);
            obj.Model = Scripts.RemoveSpaceStartEnd(obj.Model);
            obj.Category = Scripts.RemoveSpaceStartEnd(obj.Category);
            obj.NameGtrc = Scripts.RemoveSpaceStartEnd(obj.NameGtrc);
            return obj;
        }

        public Car SetNextAvailable(Car obj)
        {
            int startValue = obj.AccCarId;
            while (!IsUnique(obj))
            {
                if (obj.AccCarId < int.MaxValue) { obj.AccCarId += 1; } else { obj.AccCarId = minAccCarId; }
                if (obj.AccCarId == startValue) { break; }
            }
            return obj;
        }
    }
}
