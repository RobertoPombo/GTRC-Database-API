using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class ResultsfileService(IResultsfileContext iResultsfileContext,
        IBaseContext<Server> iServerContext,
        IBaseContext<Session> iSessionContext,
        IBaseContext<Resultsfile> iBaseContext) : BaseService<Resultsfile>(iBaseContext)
    {
        public bool Validate(Resultsfile? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Resultsfile? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            if (obj.Date > GlobalValues.DateTimeMaxValue) { obj.Date = GlobalValues.DateTimeMaxValue; isValidUniqProps = false; }
            else if (obj.Date < GlobalValues.DateTimeMinValue) { obj.Date = GlobalValues.DateTimeMinValue; isValidUniqProps = false; }
            Server? server = null;
            if (obj.Server is not null) { server = iServerContext.GetById(obj.ServerId).Result; };
            if (server is null)
            {
                List<Server> list = iServerContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Server = list[0]; obj.ServerId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Server = server; }
            Session? session = null;
            if (obj.Session is not null) { session = iSessionContext.GetById(obj.SessionId).Result; };
            if (session is null)
            {
                List<Session> list = iSessionContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Session = list[0]; obj.SessionId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Session = session; }

            int startIndexServer = 0;
            List<int> idListServer = [];
            List<Server> listServer = iServerContext.GetAll().Result;
            for (int index = 0; index < listServer.Count; index++)
            {
                idListServer.Add(listServer[index].Id);
                if (listServer[index].Id == obj.ServerId) { startIndexServer = index; }
            }
            int indexServer = startIndexServer;

            while (!await IsUnique(obj, 0))
            {
                isValidUniqProps = false;
                if (indexServer < idListServer.Count - 1) { indexServer++; }
                else { indexServer = 0; }
                obj.Server = listServer[indexServer];
                obj.ServerId = listServer[indexServer].Id;
                if (indexServer == startIndexServer)
                {
                    DateTime startDate = obj.Date;
                    if (obj.Date < GlobalValues.DateTimeMaxValue.AddDays(-1)) { obj.Date = obj.Date.AddDays(1); } else { obj.Date = GlobalValues.DateTimeMinValue; }
                    if (obj.Date == startDate) { obj = null; return false; }
                }
            }

            int startIndexSession = 0;
            List<int> idListSession = [];
            List<Session> listSession = iSessionContext.GetAll().Result;
            for (int index = 0; index < listSession.Count; index++)
            {
                idListSession.Add(listSession[index].Id);
                if (listSession[index].Id == obj.SessionId) { startIndexSession = index; }
            }
            int indexSession = startIndexSession;

            while (!await IsUnique(obj, 1))
            {
                isValidUniqProps = false;
                if (indexSession < idListSession.Count - 1) { indexSession++; }
                else { indexSession = 0; }
                obj.Session = listSession[indexSession];
                obj.SessionId = listSession[indexSession].Id;
                if (indexSession == startIndexSession)
                {
                    DateTime startDate = obj.Date;
                    if (obj.Date < GlobalValues.DateTimeMaxValue.AddDays(-1)) { obj.Date = obj.Date.AddDays(1); } else { obj.Date = GlobalValues.DateTimeMinValue; }
                    if (obj.Date == startDate) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Resultsfile?> GetTemp() { Resultsfile obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
