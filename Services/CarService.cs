using GTRC_Basics;
using GTRC_Database_API.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CarService(ICarContext iCarContext) : BaseService<Car>
    {
        private readonly int minAccCarId = 0;

        public override Car SetNextAvailable(Car car)
        {
            int startValue = car.AccCarID;
            while (!IsUnique(car))
            {
                if (car.AccCarID < int.MaxValue) { car.AccCarID += 1; } else { car.AccCarID = minAccCarId; }
                if (car.AccCarID == startValue) { break; }
            }
            return car;
        }

        public override Car Validate(Car car)
        {
            car.AccCarID = Math.Max(car.AccCarID, minAccCarId);
            car.Name = Scripts.RemoveSpaceStartEnd(car.Name);
            car.Manufacturer = Scripts.RemoveSpaceStartEnd(car.Manufacturer);
            car.Model = Scripts.RemoveSpaceStartEnd(car.Model);
            car.Category = Scripts.RemoveSpaceStartEnd(car.Category);
            car.Name_GTRC = Scripts.RemoveSpaceStartEnd(car.Name_GTRC);
            return car;
        }

        public override List<Car> GetAllAsync() { return iCarContext.GetAll().Result; }
    }
}
