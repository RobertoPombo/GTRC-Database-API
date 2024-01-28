using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EventCarService(IEventCarContext iEventCarContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Car> iCarContext,
        IBaseContext<EventCar> iBaseContext) : BaseService<EventCar>(iBaseContext)
    {
        public EventCar? Validate(EventCar? obj)
        {
            if (obj is null) { return null; }
            Event? _event = null;
            if (obj.Event is not null) { _event = iEventContext.GetById(obj.EventId).Result; };
            if (_event is null)
            {
                List<Event> list = iEventContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Event = list[0]; obj.EventId = list[0].Id; }
            }
            else { obj.Event = _event; }
            Car? car = null;
            if (obj.Car is not null) { car = iCarContext.GetById(obj.CarId).Result; };
            if (car is null)
            {
                List<Car> list = iCarContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Car = list[0]; obj.CarId = list[0].Id; }
            }
            else { obj.Car = car; }
            return obj;
        }

        public async Task<EventCar?> SetNextAvailable(EventCar? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

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
                if (indexCar < idListCar.Count - 1)
                {
                    indexCar++;
                    obj.Car = listCar[indexCar];
                    obj.CarId = listCar[indexCar].Id;
                }
                else { indexCar = 0; }
                if (indexCar == startIndexCar)
                {
                    int startIndexEvent = 0;
                    List<int> idListEvent = [];
                    List<Event> listEvent = iEventContext.GetAll().Result;
                    for (int index = 0; index < listEvent.Count; index++)
                    {
                        idListEvent.Add(listEvent[index].Id);
                        if (listEvent[index].Id == obj.EventId) { startIndexEvent = index; }
                    }
                    int indexEvent = startIndexEvent;

                    if (indexEvent < idListEvent.Count - 1)
                    {
                        indexEvent++;
                        obj.Event = listEvent[indexEvent];
                        obj.EventId = listEvent[indexEvent].Id;
                    }
                    else { indexEvent = 0; }
                    if (indexEvent == startIndexEvent) { return null; }
                }
            }

            return obj;
        }

        public async Task<EventCar?> GetTemp() { return await SetNextAvailable(new EventCar()); }
    }
}
