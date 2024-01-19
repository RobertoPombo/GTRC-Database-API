using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EventCarclassService(IEventCarclassContext iEventCarclassContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Carclass> iCarclassContext,
        IBaseContext<EventCarclass> iBaseContext) : BaseService<EventCarclass>(iBaseContext)
    {
        public EventCarclass? Validate(EventCarclass? obj)
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
            Carclass? carclass = null;
            if (obj.Carclass is not null) { carclass = iCarclassContext.GetById(obj.CarclassId).Result; };
            if (carclass is null)
            {
                List<Carclass> list = iCarclassContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Carclass = list[0]; obj.CarclassId = list[0].Id; }
            }
            else { obj.Carclass = carclass; }
            return obj;
        }

        public async Task<EventCarclass?> SetNextAvailable(EventCarclass? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int startIndexEvent = 0;
            int startIndexCarclass = 0;
            List<int> idListEvent = [];
            List<int> idListCarclass = [];
            List<Event> listEvent = iEventContext.GetAll().Result;
            List<Carclass> listCarclass = iCarclassContext.GetAll().Result;
            for (int index = 0; index < listEvent.Count; index++)
            {
                idListEvent.Add(listEvent[index].Id);
                if (listEvent[index].Id == obj.EventId) { startIndexEvent = index; }
            }
            for (int index = 0; index < listCarclass.Count; index++)
            {
                idListCarclass.Add(listCarclass[index].Id);
                if (listCarclass[index].Id == obj.CarclassId) { startIndexCarclass = index; }
            }
            int indexEvent = startIndexEvent;
            int indexCarclass = startIndexCarclass;

            while (!await IsUnique(obj))
            {
                if (indexCarclass < idListCarclass.Count - 1)
                {
                    indexCarclass++;
                    obj.Carclass = listCarclass[indexCarclass];
                    obj.CarclassId = listCarclass[indexCarclass].Id;
                }
                else { indexCarclass = 0; }
                if (indexCarclass == startIndexCarclass)
                {
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

        public async Task<EventCarclass?> GetTemp() { return await SetNextAvailable(new EventCarclass()); }

        public async Task<bool> HasChildObjects(int id)
        {
            return false;
        }
    }
}
