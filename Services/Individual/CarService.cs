using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CarService(ICarContext iCarContext,
        IBaseContext<Carclass> iCarclassContext,
        IBaseContext<Manufacturer> iManufacturerContext,
        IBaseContext<Car> iBaseContext) : BaseService<Car>(iBaseContext)
    {
        public bool Validate(Car? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Manufacturer? manufacturer = null;
            if (obj.Manufacturer is not null) { manufacturer = iManufacturerContext.GetById(obj.ManufacturerId).Result; };
            if (manufacturer is null)
            {
                List<Manufacturer> list = iManufacturerContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Manufacturer = list[0]; obj.ManufacturerId = list[0].Id; isValid = false; }
            }
            else { obj.Manufacturer = manufacturer; }
            obj.Model = Scripts.RemoveSpaceStartEnd(obj.Model);
            Carclass? carclass = null;
            if (obj.Carclass is not null) { carclass = iCarclassContext.GetById(obj.CarclassId).Result; };
            if (carclass is null)
            {
                List<Carclass> list = iCarclassContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Carclass = list[0]; obj.CarclassId = list[0].Id; isValid = false; }
            }
            else { obj.Carclass = carclass; }
            if (obj.WidthMm < Car.MinWidthMm) { obj.WidthMm = Car.MinWidthMm; isValid = false; }
            if (obj.LengthMm < Car.MinLengthMm) { obj.LengthMm = Car.MinLengthMm; isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Car? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Car.DefaultName; isValidUniqProps = false; }
            obj.NameLfm = Scripts.RemoveSpaceStartEnd(obj.NameLfm);
            obj.NameGoogleSheets = Scripts.RemoveSpaceStartEnd(obj.NameGoogleSheets);

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 0))
            {
                isValidUniqProps = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            uint startValue = obj.AccCarId;
            while (!await IsUnique(obj, 1))
            {
                isValidUniqProps = false;
                if (obj.AccCarId < uint.MaxValue) { obj.AccCarId += 1; } else { obj.AccCarId = uint.MinValue; }
                if (obj.AccCarId == startValue) { obj = null; return false; }
            }

            nr = 1;
            delimiter = " #";
            defName = obj.NameLfm;
            defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 2) && obj.NameLfm != string.Empty)
            {
                isValidUniqProps = false;
                obj.NameLfm = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj.NameLfm = string.Empty; }
            }

            nr = 1;
            delimiter = " #";
            defName = obj.NameGoogleSheets;
            defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 3) && obj.NameGoogleSheets != string.Empty)
            {
                isValidUniqProps = false;
                obj.NameGoogleSheets = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj.NameGoogleSheets = string.Empty; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Car?> GetTemp() { Car obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
