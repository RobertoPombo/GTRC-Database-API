using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeasonCarclassService(ISeasonCarclassContext iSeasonCarclassContext,
        IBaseContext<Season> iSeasonContext,
        IBaseContext<Carclass> iCarclassContext,
        IBaseContext<SeasonCarclass> iBaseContext) : BaseService<SeasonCarclass>(iBaseContext)
    {
        public bool Validate(SeasonCarclass? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            Season? season = null;
            if (obj.Season is not null) { season = iSeasonContext.GetById(obj.SeasonId).Result; };
            if (season is null)
            {
                List<Season> list = iSeasonContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Season = list[0]; obj.SeasonId = list[0].Id; isValid = false; }
            }
            else { obj.Season = season; }
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

        public async Task<bool> SetNextAvailable(SeasonCarclass? obj)
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
                    int startIndexSeason = 0;
                    List<int> idListSeason = [];
                    List<Season> listSeason = iSeasonContext.GetAll().Result;
                    for (int index = 0; index < listSeason.Count; index++)
                    {
                        idListSeason.Add(listSeason[index].Id);
                        if (listSeason[index].Id == obj.SeasonId) { startIndexSeason = index; }
                    }
                    int indexSeason = startIndexSeason;

                    if (indexSeason < idListSeason.Count - 1)
                    {
                        indexSeason++;
                        obj.Season = listSeason[indexSeason];
                        obj.SeasonId = listSeason[indexSeason].Id;
                    }
                    else { indexSeason = 0; }
                    if (indexSeason == startIndexSeason) { obj = null; return false; }
                }
            }

            return isAvailable;
        }

        public async Task<SeasonCarclass?> GetTemp() { SeasonCarclass obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
