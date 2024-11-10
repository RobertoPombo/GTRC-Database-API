using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class PrequalifyingService(IPrequalifyingContext iPrequalifyingContext,
        IBaseContext<Performancerequirement> iPerformancerequirementContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Session> iSessionContext,
        IBaseContext<Prequalifying> iBaseContext) : BaseService<Prequalifying>(iBaseContext)
    {
        public bool Validate(Prequalifying? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Performancerequirement? performancerequirement = null;
            if (obj.Performancerequirement is not null) { performancerequirement = iPerformancerequirementContext.GetById(obj.PerformancerequirementId).Result; };
            if (performancerequirement is null)
            {
                List<Performancerequirement> list = iPerformancerequirementContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Performancerequirement = list[0]; obj.PerformancerequirementId = list[0].Id; isValid = false; }
            }
            else { obj.Performancerequirement = performancerequirement; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Prequalifying? obj)
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
            Session? session = null;
            if (obj.Session is not null) { session = iSessionContext.GetById(obj.SessionId).Result; };
            if (session is null)
            {
                List<Session> list = iSessionContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Session = list[0]; obj.SessionId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Session = session; }

            int startIndexEvent = 0;
            List<int> idListEvent = [];
            List<Event> listEvent = iEventContext.GetAll().Result;
            for (int index = 0; index < listEvent.Count; index++)
            {
                idListEvent.Add(listEvent[index].Id);
                if (listEvent[index].Id == obj.EventId) { startIndexEvent = index; }
            }
            int indexEvent = startIndexEvent;

            SessionPurpose[] listTypes = (SessionPurpose[])Enum.GetValues(typeof(SessionPurpose));
            if (listTypes.Length == 0) { obj = null; return false; }
            int startIndexType = 0;
            for (int _indexType = 0; _indexType < listTypes.Length; _indexType++) { if (obj.SessionPurpose == listTypes[_indexType]) { startIndexType = _indexType; break; } }
            int indexType = startIndexType;
            obj.SessionPurpose = listTypes[indexType];

            int startIndexSession = 0;
            List<int> idListSession = [];
            List<Session> listSession = iSessionContext.GetAll().Result;
            for (int index = 0; index < listSession.Count; index++)
            {
                idListSession.Add(listSession[index].Id);
                if (listSession[index].Id == obj.SessionId) { startIndexSession = index; }
            }
            int indexSession = startIndexSession;

            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (indexSession < idListSession.Count - 1) { indexSession++; }
                else { indexSession = 0; }
                obj.Session = listSession[indexSession];
                obj.SessionId = listSession[indexSession].Id;
                if (indexSession == startIndexSession)
                {
                    if (indexType < listTypes.Length - 1) { indexType++; }
                    else { indexType = 0; }
                    obj.SessionPurpose = listTypes[indexType];
                    if (indexType == startIndexType)
                    {
                        if (indexEvent < idListEvent.Count - 1) { indexEvent++; }
                        else { indexEvent = 0; }
                        obj.Event = listEvent[indexEvent];
                        obj.EventId = listEvent[indexEvent].Id;
                        if (indexEvent == startIndexEvent) { obj = null; return false; }
                    }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Prequalifying?> GetTemp() { Prequalifying obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
