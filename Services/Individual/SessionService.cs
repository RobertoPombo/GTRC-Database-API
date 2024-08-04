using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.EfcContext;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SessionService(ISessionContext iSessionContext,
        IBaseContext<Event> iEventContext,
        IBaseContext<Pointssystem> iPointssystemContext,
        IBaseContext<StintAnalysisMethod> iStintAnalysisMethodContext,
        IBaseContext<Session> iBaseContext) : BaseService<Session>(iBaseContext)
    {
        public bool Validate(Session? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.DurationMin < Session.MinDurationMin) { obj.DurationMin = Session.MinDurationMin; isValid = false; }
            if (obj.SessionsCount < Session.MinSessionsCount) { obj.SessionsCount = Session.MinSessionsCount; isValid = false; }
            if (GlobalValues.DateTimeMaxValue - TimeSpan.FromMinutes(obj.StartTimeOffsetMin + (obj.SessionsCount * obj.DurationMin)) < obj.Event.Date)
            {
                obj.SessionsCount = Session.MinSessionsCount;
                obj.DurationMin = (ushort)Math.Min(Math.Abs((GlobalValues.DateTimeMaxValue - obj.Event.Date).TotalMinutes - (obj.SessionsCount * obj.StartTimeOffsetMin)), ushort.MaxValue);
                isValid = false;
            }
            StintAnalysisMethod? stintAnalysisMethod = null;
            if (obj.StintAnalysisMethod is not null) { stintAnalysisMethod = iStintAnalysisMethodContext.GetById(obj.StintAnalysisMethodId).Result; };
            if (stintAnalysisMethod is null)
            {
                List<StintAnalysisMethod> list = iStintAnalysisMethodContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.StintAnalysisMethod = list[0]; obj.StintAnalysisMethodId = list[0].Id; isValid = false; }
            }
            else { obj.StintAnalysisMethod = stintAnalysisMethod; }
            Pointssystem? pointssystem = null;
            if (obj.Pointssystem is not null) { pointssystem = iPointssystemContext.GetById(obj.PointssystemId).Result; };
            if (pointssystem is null)
            {
                List<Pointssystem> list = iPointssystemContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Pointssystem = list[0]; obj.PointssystemId = list[0].Id; isValid = false; }
            }
            else { obj.Pointssystem = pointssystem; }
            if (obj.PreviousSessionId != GlobalValues.NoId)
            {
                Session? previousSession = GetById(obj.PreviousSessionId).Result;
                if (previousSession is null || obj.EventId != previousSession.EventId || obj.StartTimeOffsetMin <= previousSession.StartTimeOffsetMin)
                {
                    obj.PreviousSessionId = GlobalValues.NoId;
                    isValid = false;
                }
            }
            if (obj.GridSessionId != GlobalValues.NoId)
            {
                Session? gridSession = GetById(obj.GridSessionId).Result;
                if (gridSession is null || obj.EventId != gridSession.EventId || obj.StartTimeOffsetMin <= gridSession.StartTimeOffsetMin)
                {
                    obj.GridSessionId = GlobalValues.NoId;
                    isValid = false;
                }
            }
            if (obj.ReverseGridFrom < Session.MinReverseGridFrom) { obj.ReverseGridFrom = Session.MinReverseGridFrom; isValid = false; }
            if (obj.ReverseGridFrom > obj.ReverseGridTo) { obj.ReverseGridTo = obj.ReverseGridFrom; isValid = false; }
            if (obj.DayOfWeekend < Session.MinDayOfWeekend) { obj.DayOfWeekend = Session.MinDayOfWeekend; isValid = false; }
            if (obj.DayOfWeekend > Session.MaxDayOfWeekend) { obj.DayOfWeekend = Session.MaxDayOfWeekend; isValid = false; }
            if (obj.TimeMultiplier < Session.MinTimeMultiplier) { obj.TimeMultiplier = Session.MinTimeMultiplier; isValid = false; }
            if (obj.TimeMultiplier > Session.MaxTimeMultiplier) { obj.TimeMultiplier = Session.MaxTimeMultiplier; isValid = false; }
            if (obj.EntrylistType == EntrylistType.None && obj.ForceEntrylist) { obj.ForceEntrylist = false; isValid = false; }
            if (obj.EntrylistType != EntrylistType.Season && obj.ForceCarModel) { obj.ForceCarModel = false; isValid = false; }
            obj.ServerName = Scripts.RemoveSpaceStartEnd(obj.ServerName);
            obj.DriverPassword = Scripts.RemoveSpaceStartEnd(obj.DriverPassword);
            obj.SpectatorPassword = Scripts.RemoveSpaceStartEnd(obj.SpectatorPassword);
            obj.AdminPassword = Scripts.RemoveSpaceStartEnd(obj.AdminPassword);
            if (obj.ServerName == string.Empty) { obj.ServerName = Session.DefaultServerName; isValid = false; }
            if (obj.DriverPassword != string.Empty && obj.DriverPassword.Length < Session.MinCharacterCountPassword) { obj.DriverPassword = string.Empty; isValid = false; }
            if (obj.SpectatorPassword != string.Empty && obj.SpectatorPassword.Length < Session.MinCharacterCountPassword) { obj.SpectatorPassword = string.Empty; isValid = false; }
            if (obj.AdminPassword != string.Empty && obj.AdminPassword.Length < Session.MinCharacterCountPassword) { obj.AdminPassword = string.Empty; isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Session? obj)
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
            if (GlobalValues.DateTimeMaxValue - TimeSpan.FromMinutes(obj.StartTimeOffsetMin + Session.MinSessionsCount) < obj.Event.Date)
            {
                obj.StartTimeOffsetMin = (ushort)Math.Min(Math.Abs((GlobalValues.DateTimeMaxValue - obj.Event.Date).TotalMinutes - Session.MinSessionsCount), ushort.MaxValue);
                isValidUniqProps = false;
            }

            int startIndexEvent = 0;
            List<int> idListEvent = [];
            List<Event> listEvent = iEventContext.GetAll().Result;
            for (int index = 0; index < listEvent.Count; index++)
            {
                idListEvent.Add(listEvent[index].Id);
                if (listEvent[index].Id == obj.EventId) { startIndexEvent = index; }
            }
            int indexEvent = startIndexEvent;

            int startStartTimeOffsetMin = obj.StartTimeOffsetMin;
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (GlobalValues.DateTimeMaxValue - TimeSpan.FromMinutes(obj.StartTimeOffsetMin + Session.MinSessionsCount + 1) < obj.Event.Date) { obj.StartTimeOffsetMin += 1; }
                else { obj.StartTimeOffsetMin = ushort.MinValue; }
                if (obj.StartTimeOffsetMin == startStartTimeOffsetMin)
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

        public async Task<Session?> GetTemp() { Session obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
