using System.Net;

using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EventCarService(IEventCarContext iEventCarContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Car> iCarContext,
        BaseService<BopTrackCar> bopTrackCarService,
        CarService carService,
        EntryService entryService,
        EntryDatetimeService entryDatetimeService,
        EntryEventService entryEventService,
        IBaseContext<EventCar> iBaseContext) : BaseService<EventCar>(iBaseContext)
    {
        public bool Validate(EventCar? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(EventCar? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Event? _event = null;
            if (obj.Event is not null) { _event = iEventContext.GetById(obj.EventId).Result; };
            if (_event is null)
            {
                List<Event> list = iEventContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Event = list[0]; obj.EventId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Event = _event; }
            Car? car = null;
            if (obj.Car is not null) { car = iCarContext.GetById(obj.CarId).Result; };
            if (car is null)
            {
                List<Car> list = iCarContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Car = list[0]; obj.CarId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Car = car; }

            int startIndexEvent = 0;
            List<int> idListEvent = [];
            List<Event> listEvent = iEventContext.GetAll().Result;
            for (int index = 0; index < listEvent.Count; index++)
            {
                idListEvent.Add(listEvent[index].Id);
                if (listEvent[index].Id == obj.EventId) { startIndexEvent = index; }
            }
            int indexEvent = startIndexEvent;

            int startIndexCar = 0;
            List<int> idListCar = [];
            List<Car> listCar = iCarContext.GetAll().Result;
            for (int index = 0; index < listCar.Count; index++)
            {
                idListCar.Add(listCar[index].Id);
                if (listCar[index].Id == obj.CarId) { startIndexCar = index; }
            }
            int indexCar = startIndexCar;

            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (indexCar < idListCar.Count - 1) { indexCar++; }
                else { indexCar = 0; }
                obj.Car = listCar[indexCar];
                obj.CarId = listCar[indexCar].Id;
                if (indexCar == startIndexCar)
                {
                    if (indexEvent < idListEvent.Count - 1) { indexEvent++; }
                    else { indexEvent = 0; }
                    obj.Event = listEvent[indexEvent];
                    obj.EventId = listEvent[indexEvent].Id;
                    if (indexEvent == startIndexEvent) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<EventCar?> GetTemp() { EventCar obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<(List<EventCar>, List<EventCar>)> CountCars(Event _event)
        {
            List<EventCar> eventCarsToBeAdded = [];
            List<EventCar> eventCarsToBeUpdated = [];
            Season season = _event.Season;
            List<EventCar> listEventCars = await GetChildObjects(typeof(Event), _event.Id);
            foreach (EventCar _eventCar in listEventCars)
            {
                EventCar _default = new();
                _eventCar.BallastKg = _default.BallastKg;
                _eventCar.Restrictor = _default.Restrictor;
                _eventCar.Count = _default.Count;
                _eventCar.CountBop = _default.CountBop;
                eventCarsToBeUpdated.Add(_eventCar);
            }
            List<Entry> listEntries = await entryService.GetChildObjects(typeof(Season), season.Id);
            for (int index1 = 0; index1 < listEntries.Count - 1; index1++)
            {
                for (int index2 = index1 + 1; index2 < listEntries.Count; index2++)
                {
                    if (await entryService.GetDateLatestCarChange(listEntries[index1], DateTime.UtcNow) > await entryService.GetDateLatestCarChange(listEntries[index2], DateTime.UtcNow))
                    {
                        (listEntries[index1], listEntries[index2]) = (listEntries[index2], listEntries[index1]);
                    }
                }
            }
            foreach (Entry entry in listEntries)
            {
                (HttpStatusCode statusEntEve, EntryEvent? entryEvent) = await entryEventService.GetAnyByUniqProps(new() { EntryId = entry.Id, EventId = _event.Id });
                if (statusEntEve == HttpStatusCode.OK && entry.RegisterDate < _event.Date && entry.IsPointScorer)
                {
                    DateTime carChangeDateMax = _event.Date;
                    if (season.DateBopFreeze < _event.Date) { carChangeDateMax = season.DateBopFreeze; }
                    Car car = entry.Car;
                    Car carAtFreeze = entry.Car;
                    (HttpStatusCode statusEntDat, EntryDatetime? entryDatetime) = await entryDatetimeService.GetAnyByUniqProps(new() { EntryId = entry.Id, Date = _event.Date });
                    (HttpStatusCode statusEntDatAtFreeze, EntryDatetime? entryDatetimeAtFreeze) = await entryDatetimeService.GetAnyByUniqProps(new() { EntryId = entry.Id, Date = carChangeDateMax });
                    if (statusEntDat == HttpStatusCode.OK && entryDatetime is not null) { car = entryDatetime.Car; }
                    if (statusEntDatAtFreeze == HttpStatusCode.OK && entryDatetimeAtFreeze is not null) { carAtFreeze = entryDatetimeAtFreeze.Car; }
                    DateTime carChangeDate = await entryService.GetDateLatestCarChange(entry, _event.Date);
                    DateTime carChangeDateAtFreeze = await entryService.GetDateLatestCarChange(entry, carChangeDateMax);
                    EventCar eventCar = new() { EventId = _event.Id, Event = _event, CarId = car.Id, Car = car };
                    EventCar? eventCarNullable = null;
                    foreach (EventCar _eventCar in listEventCars) { if (_eventCar.EventId == _event.Id && _eventCar.CarId == car.Id) { eventCarNullable = _eventCar; break; } }
                    if (eventCarNullable is null) { eventCarsToBeAdded.Add(eventCar); listEventCars.Add(eventCar); }
                    else { eventCar = eventCarNullable; }
                    EventCar eventCarAtFreeze = new() { EventId = _event.Id, Event = _event, CarId = carAtFreeze.Id, Car = carAtFreeze };
                    EventCar? eventCarAtFreezeNullable = null;
                    foreach (EventCar _eventCar in listEventCars) { if (_eventCar.EventId == _event.Id && _eventCar.CarId == carAtFreeze.Id) { eventCarAtFreezeNullable = _eventCar; break; } }
                    if (eventCarAtFreezeNullable is null) { eventCarsToBeAdded.Add(eventCarAtFreeze); listEventCars.Add(eventCarAtFreeze); }
                    else { eventCarAtFreeze = eventCarAtFreezeNullable; }
                    int carCount = eventCar.Count ;
                    int carCountBop = eventCarAtFreeze.CountBop;
                    if (season.GroupCarRegistrationLimits)
                    {
                        carCount = 0;
                        carCountBop = 0;
                        foreach (EventCar _eventCar in listEventCars)
                        {
                            Car _car = _eventCar.Car;
                            if (car.ManufacturerId == _car.ManufacturerId && car.CarclassId == _car.CarclassId) { carCount += _eventCar.CountBop; }
                            if (carAtFreeze.ManufacturerId == _car.ManufacturerId && carAtFreeze.CarclassId == _car.CarclassId) { carCountBop += _eventCar.CountBop; }
                        }
                    }
                    bool respectsRegLimit = carChangeDate < season.DateStartCarRegistrationLimit || carCount < season.CarRegistrationLimit;
                    bool respectsRegLimitAtFreeze0 = carChangeDateAtFreeze < season.DateStartCarRegistrationLimit;
                    bool respectsRegLimitAtFreeze = respectsRegLimitAtFreeze0 || carCountBop < season.CarRegistrationLimit;
                    bool isRegistered = entry.SignOutDate > _event.Date;
                    bool isRegisteredAtFreeze0 = entry.RegisterDate < season.DateBopFreeze;
                    bool isRegisteredAtFreeze = isRegisteredAtFreeze0 && (entry.SignOutDate > season.DateBopFreeze || entry.SignOutDate > _event.Date);
                    if (isRegisteredAtFreeze && respectsRegLimitAtFreeze) { eventCarAtFreeze.CountBop++; }
                    if (isRegistered && respectsRegLimit) { eventCarAtFreeze.Count++; }  //not yet implemented: Hier war im CommunityManager eine Änderung der EventsEntries vorgesehen
                }
            }
            return (eventCarsToBeAdded, eventCarsToBeUpdated);
        }

        public async Task<List<EventCar>> CalculateBop(List<EventCar> listEventCars)
        {
            foreach (EventCar eventCar in listEventCars)
            {
                int carCount = eventCar.CountBop;
                bool isLatestModel = await carService.GetIsLatestModel(eventCar.Car);
                if (eventCar.Event.Season.GroupCarRegistrationLimits)
                {
                    carCount = 0;
                    foreach (EventCar _eventCar in listEventCars)
                    {
                        if (eventCar.Car.ManufacturerId == _eventCar.Car.ManufacturerId && eventCar.Car.CarclassId == _eventCar.Car.CarclassId)
                        {
                            carCount += _eventCar.CountBop;
                        }
                    }
                }
                if (!eventCar.Event.Season.BopLatestModelOnly || isLatestModel)
                {
                    int ballastKg = Math.Max(0, carCount - eventCar.Event.Season.CarLimitBallast) * eventCar.Event.Season.GainBallast;
                    int restrictor = Math.Max(0, carCount - eventCar.Event.Season.CarLimitRestrictor) * eventCar.Event.Season.GainRestrictor;
                    eventCar.BallastKg = (short)Math.Min(Math.Max(ballastKg, short.MinValue), short.MaxValue);
                    eventCar.Restrictor = (short)Math.Min(Math.Max(restrictor, short.MinValue), short.MaxValue);
                }
                else
                {
                    Entry _default = new();
                    eventCar.BallastKg = _default.BallastKg;
                    eventCar.Restrictor = _default.Restrictor;
                }
                BopTrackCarUniqPropsDto0 uniqDtoBopTraCar = new() { BopId = eventCar.Event.Season.BopId, TrackId = eventCar.Event.TrackId, CarId = eventCar.CarId };
                BopTrackCar? bopTrackCar = await bopTrackCarService.GetByUniqProps(new() { Dto = uniqDtoBopTraCar });
                if (bopTrackCar is not null)
                {
                    eventCar.BallastKg += bopTrackCar.BallastKg;
                    eventCar.Restrictor += bopTrackCar.Restrictor;
                }
            }
            return listEventCars;
        }
    }
}
