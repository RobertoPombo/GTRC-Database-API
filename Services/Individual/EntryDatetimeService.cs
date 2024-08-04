using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EntryDatetimeService(IEntryDatetimeContext iEntryDatetimeContext,
        IBaseContext<Car> iCarContext,
        IBaseContext<Entry> iEntryContext,
        IBaseContext<EntryDatetime> iBaseContext) : BaseService<EntryDatetime>(iBaseContext)
    {
        public bool Validate(EntryDatetime? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Car? car = null;
            if (obj.Car is not null) { car = iCarContext.GetById(obj.CarId).Result; };
            if (car is null)
            {
                List<Car> list = iCarContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Car = list[0]; obj.CarId = list[0].Id; isValid = false; }
            }
            else { obj.Car = car; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(EntryDatetime? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Entry? entry = null;
            if (obj.Entry is not null) { entry = iEntryContext.GetById(obj.EntryId).Result; };
            if (entry is null)
            {
                List<Entry> list = iEntryContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Entry = list[0]; obj.EntryId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Entry = entry; }
            if (obj.Date > GlobalValues.DateTimeMaxValue) { obj.Date = GlobalValues.DateTimeMaxValue; isValidUniqProps = false; }
            else if (obj.Date < GlobalValues.DateTimeMinValue) { obj.Date = GlobalValues.DateTimeMinValue; isValidUniqProps = false; }

            int startIndexEntry = 0;
            List<int> idListEntry = [];
            List<Entry> listEntry = iEntryContext.GetAll().Result;
            for (int index = 0; index < listEntry.Count; index++)
            {
                idListEntry.Add(listEntry[index].Id);
                if (listEntry[index].Id == obj.EntryId) { startIndexEntry = index; }
            }
            int indexEntry = startIndexEntry;

            DateTime startDate = obj.Date;
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (obj.Date < GlobalValues.DateTimeMaxValue.AddDays(-1)) { obj.Date = obj.Date.AddDays(1); } else { obj.Date = GlobalValues.DateTimeMinValue; }
                if (obj.Date == startDate)
                {
                    if (indexEntry < idListEntry.Count - 1) { indexEntry++; }
                    else { indexEntry = 0; }
                    obj.Entry = listEntry[indexEntry];
                    obj.EntryId = listEntry[indexEntry].Id;
                    if (indexEntry == startIndexEntry) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<EntryDatetime?> GetTemp() { EntryDatetime obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
