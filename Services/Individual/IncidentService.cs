using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class IncidentService(IIncidentContext iIncidentContext,
        IBaseContext<Session> iSessionContext,
        IBaseContext<Incident> iBaseContext) : BaseService<Incident>(iBaseContext)
    {
        public bool Validate(Incident? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.IsApplied && obj.Status != IncidentStatus.DoneLive && obj.Status != IncidentStatus.DonePostRace) { obj.IsApplied = false; isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Incident? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Session? session = null;
            if (obj.Session is not null) { session = iSessionContext.GetById(obj.SessionId).Result; };
            if (session is null)
            {
                List<Session> list = iSessionContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Session = list[0]; obj.SessionId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Session = session; }

            int startIndexSession = 0;
            List<int> idListSession = [];
            List<Session> listSession = iSessionContext.GetAll().Result;
            for (int index = 0; index < listSession.Count; index++)
            {
                idListSession.Add(listSession[index].Id);
                if (listSession[index].Id == obj.SessionId) { startIndexSession = index; }
            }
            int indexSession = startIndexSession;

            uint startValue = obj.TimeStampMs;
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (obj.TimeStampMs < uint.MaxValue) { obj.TimeStampMs += 1; } else { obj.TimeStampMs = uint.MinValue; }
                if (obj.TimeStampMs == startValue)
                {
                    isValidUniqProps = false;
                    if (indexSession < idListSession.Count - 1) { indexSession++; }
                    else { indexSession = 0; }
                    obj.Session = listSession[indexSession];
                    obj.SessionId = listSession[indexSession].Id;
                    if (indexSession == startIndexSession) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Incident?> GetTemp() { Incident obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
