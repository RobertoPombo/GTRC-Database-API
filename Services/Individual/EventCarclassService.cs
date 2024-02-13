using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class EventCarclassService(IEventCarclassContext iEventCarclassContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Carclass> iCarclassContext,
        IBaseContext<EventCarclass> iBaseContext) : BaseService<EventCarclass>(iBaseContext)
    {
        public bool Validate(EventCarclass? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Event? _event = null;
            if (obj.Event is not null) { _event = iEventContext.GetById(obj.EventId).Result; };
            if (_event is null)
            {
                List<Event> list = iEventContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Event = list[0]; obj.EventId = list[0].Id; isValid = false; }
            }
            else { obj.Event = _event; }
            Carclass? carclass = null;
            if (obj.Carclass is not null) { carclass = iCarclassContext.GetById(obj.CarclassId).Result; };
            if (carclass is null)
            {
                List<Carclass> list = iCarclassContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Carclass = list[0]; obj.CarclassId = list[0].Id; isValid = false; }
            }
            else { obj.Carclass = carclass; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(EventCarclass? obj)
        {
            bool isAvailable = true;
            if (obj is null) { return false; }

            int startIndexCarclass = 0;
            List<int> idListCarclass = [];
            List<Carclass> listCarclass = iCarclassContext.GetAll().Result;
            for (int index = 0; index < listCarclass.Count; index++)
            {
                idListCarclass.Add(listCarclass[index].Id);
                if (listCarclass[index].Id == obj.CarclassId) { startIndexCarclass = index; }
            }
            int indexCarclass = startIndexCarclass;

            while (!await IsUnique(obj))
            {
                isAvailable = false;
                if (indexCarclass < idListCarclass.Count - 1)
                {
                    indexCarclass++;
                    obj.Carclass = listCarclass[indexCarclass];
                    obj.CarclassId = listCarclass[indexCarclass].Id;
                }
                else { indexCarclass = 0; }
                if (indexCarclass == startIndexCarclass)
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
                    if (indexEvent == startIndexEvent) { obj = null; return false; }
                }
            }

            return isAvailable;
        }

        public async Task<EventCarclass?> GetTemp() { EventCarclass obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
