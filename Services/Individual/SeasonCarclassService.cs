using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeasonCarclassService(ISeasonCarclassContext iSeasonCarclassContext, IBaseContext<Season> iSeasonContext, IBaseContext<Carclass> iCarclassContext, IBaseContext<SeasonCarclass> iBaseContext) : BaseService<SeasonCarclass>(iBaseContext)
    {
        public SeasonCarclass? Validate(SeasonCarclass? obj)
        {
            if (obj is null) { return null; }
            Season? season = null;
            if (obj.Season is not null) { season = iSeasonContext.GetById(obj.SeasonId).Result; };
            if (season is null)
            {
                List<Season> list = iSeasonContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Season = list[0]; obj.SeasonId = list[0].Id; }
            }
            else { obj.Season = season; }
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

        public async Task<SeasonCarclass?> SetNextAvailable(SeasonCarclass? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int startIndexSeason = 0;
            int startIndexCarclass = 0;
            List<int> idListSeason = [];
            List<int> idListCarclass = [];
            List<Season> listSeason = iSeasonContext.GetAll().Result;
            List<Carclass> listCarclass = iCarclassContext.GetAll().Result;
            for (int index = 0; index < listSeason.Count; index++)
            {
                idListSeason.Add(listSeason[index].Id);
                if (listSeason[index].Id == obj.SeasonId) { startIndexSeason = index; }
            }
            for (int index = 0; index < listCarclass.Count; index++)
            {
                idListCarclass.Add(listCarclass[index].Id);
                if (listCarclass[index].Id == obj.CarclassId) { startIndexCarclass = index; }
            }
            int indexSeason = startIndexSeason;
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
                    if (indexSeason < idListSeason.Count - 1)
                    {
                        indexSeason++;
                        obj.Season = listSeason[indexSeason];
                        obj.SeasonId = listSeason[indexSeason].Id;
                    }
                    else { indexSeason = 0; }
                    if (indexSeason == startIndexSeason) { return null; }
                }
            }

            return obj;
        }

        public async Task<SeasonCarclass?> GetTemp() { return await SetNextAvailable(new SeasonCarclass()); }

        public async Task<bool> HasChildObjects(int id)
        {
            return false;
        }
    }
}
