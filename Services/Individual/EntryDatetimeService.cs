using System.Net;

using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;
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

        public async Task<(HttpStatusCode, EntryDatetime?)> GetAnyByUniqProps(EntryDatetimeUniqPropsDto0 objDto)
        {
            EntryDatetimeAddDto _objDtoDto = Scripts.Map(objDto, new EntryDatetimeAddDto());
            _objDtoDto.Date = null;
            AddDto<EntryDatetime> _objDto = new() { Dto = _objDtoDto };
            List<EntryDatetime> list = Scripts.SortByDate(await GetByProps(_objDto));
            if (list.Count == 0 || list[0].Date > objDto.Date)
            {
                Entry? entry = await iEntryContext.GetById(objDto.EntryId);
                if (entry is null) { return (HttpStatusCode.NotFound, null); }
                else
                {
                    EntryDatetime newObj = new() { EntryId = entry.Id, Date = entry.RegisterDate, CarId = entry.CarId };
                    await ValidateUniqProps(newObj);
                    if (newObj is not null) { return (HttpStatusCode.OK, newObj); }
                    else { return (HttpStatusCode.NotAcceptable, newObj); }
                }
            }
            else
            {
                for (int index = 0; index < list.Count - 1; index++) { if (list[index + 1].Date > objDto.Date) { return (HttpStatusCode.OK, list[index]); } }
                return (HttpStatusCode.OK, list[^1]);
            }
        }
    }
}
