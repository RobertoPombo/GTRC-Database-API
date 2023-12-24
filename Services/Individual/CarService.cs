using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CarService(ICarContext iCarContext, IBaseContext<Car> iBaseContext) : BaseService<Car>(iBaseContext)
    {
        public static Car Validate(Car obj)
        {
            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            obj.Manufacturer = Scripts.RemoveSpaceStartEnd(obj.Manufacturer);
            obj.Model = Scripts.RemoveSpaceStartEnd(obj.Model);
            if (!Enum.IsDefined(typeof(CarClass), obj.Class)) { obj.Class = CarClass.General; }
            obj.WidthMm = Math.Max(obj.WidthMm, (ushort)1);
            obj.LengthMm = Math.Max(obj.LengthMm, (ushort)1);
            obj.NameGtrc = Scripts.RemoveSpaceStartEnd(obj.NameGtrc);
            return obj;
        }

        public async Task<Car?> SetNextAvailable(Car obj)
        {
            uint startValue = obj.AccCarId;
            while (!await IsUnique(obj))
            {
                if (obj.AccCarId < uint.MaxValue) { obj.AccCarId += 1; } else { obj.AccCarId = uint.MinValue; }
                if (obj.AccCarId == startValue) { return null; }
            }
            return obj;
        }

        public async Task<Car?> GetTemp() { return await SetNextAvailable(Validate(new Car())); }
    }
}
