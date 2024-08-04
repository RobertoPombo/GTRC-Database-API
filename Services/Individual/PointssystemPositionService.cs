using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class PointssystemPositionService(IPointssystemPositionContext iPointssystemPositionContext,
        IBaseContext<Pointssystem> iPointssystemContext,
        IBaseContext<PointssystemPosition> iBaseContext) : BaseService<PointssystemPosition>(iBaseContext)
    {
        public bool Validate(PointssystemPosition? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(PointssystemPosition? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Pointssystem? pointssystem = null;
            if (obj.Pointssystem is not null) { pointssystem = iPointssystemContext.GetById(obj.PointssystemId).Result; };
            if (pointssystem is null)
            {
                List<Pointssystem> list = iPointssystemContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Pointssystem = list[0]; obj.PointssystemId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Pointssystem = pointssystem; }
            if (obj.Position < PointssystemPosition.MinPosition) { obj.Position = PointssystemPosition.MinPosition; isValidUniqProps = false; }

            int startIndexPointssystem = 0;
            List<int> idListPointssystem = [];
            List<Pointssystem> listPointssystem = iPointssystemContext.GetAll().Result;
            for (int index = 0; index < listPointssystem.Count; index++)
            {
                idListPointssystem.Add(listPointssystem[index].Id);
                if (listPointssystem[index].Id == obj.PointssystemId) { startIndexPointssystem = index; }
            }
            int indexPointssystem = startIndexPointssystem;

            byte startPosition = obj.Position;
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (obj.Position < byte.MaxValue) { obj.Position += 1; } else { obj.Position = PointssystemPosition.MinPosition; }
                if (obj.Position == startPosition)
                {
                    if (indexPointssystem < idListPointssystem.Count - 1) { indexPointssystem++; }
                    else { indexPointssystem = 0; }
                    obj.Pointssystem = listPointssystem[indexPointssystem];
                    obj.PointssystemId = listPointssystem[indexPointssystem].Id;
                    if (indexPointssystem == startIndexPointssystem) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<PointssystemPosition?> GetTemp() { PointssystemPosition obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
