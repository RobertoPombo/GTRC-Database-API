using System.Reflection;

using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.DTOs;
using GTRC_Database_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            if (!Enum.IsDefined(typeof(CarClass), obj.Class)) { obj.Class = CarClass.General; }
            obj.WidthMm = Math.Max(obj.WidthMm, 1);
            obj.LengthMm = Math.Max(obj.LengthMm, 1);
            obj.NameGtrc = Scripts.RemoveSpaceStartEnd(obj.NameGtrc);
            return obj;
        }

        public async Task<Car?> SetNextAvailable(Car obj)
        {
            int startValue = obj.AccCarId;
            while (!await IsUnique(obj))
            {
                if (obj.AccCarId < int.MaxValue) { obj.AccCarId += 1; } else { obj.AccCarId = minAccCarId; }
                if (obj.AccCarId == startValue) { return null; }
            }
            return obj;
        }

        public async Task<Car?> GetByUniqProps(CarUniqPropsDto0 objDto)
        {
            List<dynamic> listValues = [];
            for (int propertyNr = 0; propertyNr < UniqProps[0].Count; propertyNr++)
            {
                listValues.Add(Scripts.GetCastedValue(objDto.Map(), UniqProps[0][propertyNr]));
            }
            return await GetByUniqProps(listValues);
        }

        public async Task<List<Car>> GetBy(CarAddDto objDto)
        {
            List<PropertyInfo> properties = [];
            List<dynamic> values = [];
            foreach (PropertyInfo property in objDto.GetType().GetProperties())
            {
                if (property.GetValue(objDto) is not null)
                {
                    foreach (PropertyInfo baseProperty in typeof(Car).GetProperties())
                    {
                        if (property.Name == baseProperty.Name)
                        {
                            properties.Add(baseProperty);
                            values.Add(Scripts.GetCastedValue(objDto, property));
                            break;
                        }
                    }
                }
            }
            return await GetBy(properties, values);
        }

        public async Task<List<Car>> GetByFilter(CarFilterDto objDto, CarFilterDto objDtoMin, CarFilterDto objDtoMax)
        {
            List<Car> list = await GetAll();
            foreach (Car obj in list)
            {

            }
            return list;
        }

        public async Task<Car?> GetTemp() { return await SetNextAvailable(new Car()); }
    }
}
