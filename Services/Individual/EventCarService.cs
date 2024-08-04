using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EventCarService(IEventCarContext iEventCarContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Car> iCarContext,
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
    }
}
