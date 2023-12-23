using System.Reflection;

using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Helpers;
using GTRC_Database_API.Services.DTOs;
using GTRC_Database_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;

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

        public async Task<List<Car>> GetByProps(CarAddDto objDto)
        {
            List<PropertyInfo> properties = [];
            List<dynamic> values = [];
            foreach (PropertyInfo filterProperty in typeof(CarAddDto).GetProperties())
            {
                if (filterProperty.GetValue(objDto) is not null)
                {
                    foreach (PropertyInfo property in typeof(Car).GetProperties())
                    {
                        if (filterProperty.Name == property.Name)
                        {
                            properties.Add(property);
                            values.Add(Scripts.GetCastedValue(objDto, filterProperty));
                            break;
                        }
                    }
                }
            }
            return await GetByProps(properties, values);
        }

        public async Task<List<Car>> GetByFilter(CarFilterDtos objDto)
        {
            List<string> numericalTypes = ["System.Int16", "System.Int32", "System.Int64", "System.UInt16", "System.UInt32", "System.UInt64", "System.Single", "System.Double", "System.Decimal", "System.DateTime"];
            List<Car> list = await GetAll();
            List<Car> filteredList = [];
            foreach (Car obj in list)
            {
                bool isInList = true;
                foreach (PropertyInfo filterProperty in typeof(CarFilterDto).GetProperties())
                {
                    var filter = filterProperty.GetValue(objDto.Filter);
                    var filterMin = filterProperty.GetValue(objDto.FilterMin);
                    var filterMax = filterProperty.GetValue(objDto.FilterMax);
                    if (filter is not null || filterMin is not null || filterMax is not null)
                    {
                        string strFilter = filter?.ToString()?.ToLower() ?? "";
                        string strFilterMin = filterMin?.ToString()?.ToLower() ?? "";
                        string strFilterMax = filterMax?.ToString()?.ToLower() ?? "";
                        foreach (PropertyInfo property in typeof(Car).GetProperties())
                        {
                            if (filterProperty.Name == property.Name)
                            {
                                var castedValue = Scripts.GetCastedValue(obj, property);
                                string strValue = castedValue.ToString().ToLower();
                                if (!strValue.Contains(strFilter)) { isInList = false; }
                                else if (numericalTypes.Contains(property.PropertyType.ToString()))
                                {
                                    if (filterMin is not null)
                                    {
                                        var castedFilterMin = Scripts.CastValue(property, filterMin);
                                        if (castedValue < castedFilterMin) { isInList = false; }
                                    }
                                    if (filterMax is not null)
                                    {
                                        var castedFilterMax = Scripts.CastValue(property, filterMax);
                                        if (castedValue > castedFilterMax) { isInList = false; }
                                    }
                                }
                                else if ((strFilterMin.Length > 0 && string.Compare(strValue, strFilterMin) == -1)
                                    || (strFilterMax.Length > 0 && string.Compare(strValue, strFilterMax) == 1))
                                {
                                    isInList = false;
                                }
                                break;
                            }
                        }
                        if (!isInList) { break; }
                    }
                }
                if (isInList) { filteredList.Add(obj); }
            }
            return filteredList;
        }

        public async Task<Car?> GetTemp() { return await SetNextAvailable(new Car()); }
    }
}
