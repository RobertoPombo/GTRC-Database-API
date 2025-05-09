﻿using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class CarclassService(ICarclassContext iCarclassContext,
        EventService eventService,
        EventCarclassService eventCarclassService,
        IBaseContext<Carclass> iBaseContext) : BaseService<Carclass>(iBaseContext)
    {
        public bool Validate(Carclass? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Carclass? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Carclass.DefaultName; isValidUniqProps = false; }

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Carclass?> GetTemp() { Carclass obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<List<Carclass>> GetBySeason(Season season)
        {
            List<Carclass> list = [];
            List<Event> listEvents = await eventService.GetChildObjects(typeof(Season), season.Id);
            foreach (Event _event in listEvents)
            {
                List<EventCarclass> listEventCarclasses = await eventCarclassService.GetChildObjects(typeof(Event), _event.Id);
                foreach (EventCarclass eventCarclass in listEventCarclasses)
                {
                    if (!Scripts.ListContainsId(list, eventCarclass.Carclass)) { list.Add(eventCarclass.Carclass); }
                }
            }
            return list;
        }
    }
}
