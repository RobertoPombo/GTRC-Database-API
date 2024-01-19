using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class BopTrackCarService(IBopTrackCarContext iBopTrackCarContext,
        IBaseContext<Bop> iBopContext,
        IBaseContext<Track> iTrackContext,
        IBaseContext<Car> iCarContext,
        IBaseContext<BopTrackCar> iBaseContext) : BaseService<BopTrackCar>(iBaseContext)
    {
        public BopTrackCar? Validate(BopTrackCar? obj)
        {
            if (obj is null) { return null; }
            Bop? bop = null;
            if (obj.Bop is not null) { bop = iBopContext.GetById(obj.BopId).Result; };
            if (bop is null)
            {
                List<Bop> list = iBopContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Bop = list[0]; obj.BopId = list[0].Id; }
            }
            else { obj.Bop = bop; }
            Track? track = null;
            if (obj.Track is not null) { track = iTrackContext.GetById(obj.TrackId).Result; };
            if (track is null)
            {
                List<Track> list = iTrackContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Track = list[0]; obj.TrackId = list[0].Id; }
            }
            else { obj.Track = track; }
            Car? car = null;
            if (obj.Car is not null) { car = iCarContext.GetById(obj.CarId).Result; };
            if (car is null)
            {
                List<Car> list = iCarContext.GetAll().Result;
                if (list.Count == 0) { return null; }
                else { obj.Car = list[0]; obj.CarId = list[0].Id; }
            }
            else { obj.Car = car; }
            return obj;
        }

        public async Task<BopTrackCar?> SetNextAvailable(BopTrackCar? obj)
        {
            obj = Validate(obj);
            if (obj is null) { return null; }

            int startIndexBop = 0;
            int startIndexTrack = 0;
            int startIndexCar = 0;
            List<int> idListBop = [];
            List<int> idListTrack = [];
            List<int> idListCar = [];
            List<Bop> listBop = iBopContext.GetAll().Result;
            List<Track> listTrack = iTrackContext.GetAll().Result;
            List<Car> listCar = iCarContext.GetAll().Result;
            for (int index = 0; index < listBop.Count; index++)
            {
                idListBop.Add(listBop[index].Id);
                if (listBop[index].Id == obj.BopId) { startIndexBop = index; }
            }
            for (int index = 0; index < listTrack.Count; index++)
            {
                idListTrack.Add(listTrack[index].Id);
                if (listTrack[index].Id == obj.TrackId) { startIndexTrack = index; }
            }
            for (int index = 0; index < listCar.Count; index++)
            {
                idListCar.Add(listCar[index].Id);
                if (listCar[index].Id == obj.CarId) { startIndexCar = index; }
            }
            int indexBop = startIndexBop;
            int indexTrack = startIndexTrack;
            int indexCar = startIndexCar;

            while (!await IsUnique(obj))
            {
                if (indexCar < idListCar.Count - 1)
                {
                    indexCar++;
                    obj.Car = listCar[indexCar];
                    obj.CarId = listCar[indexCar].Id;
                }
                else { indexCar = 0; }
                if (indexCar == startIndexCar)
                {
                    if (indexTrack < idListTrack.Count - 1)
                    {
                        indexTrack++;
                        obj.Track = listTrack[indexTrack];
                        obj.TrackId = listTrack[indexTrack].Id;
                    }
                    else { indexTrack = 0; }
                    if (indexTrack == startIndexTrack)
                    {
                        if (indexBop < idListBop.Count - 1)
                        {
                            indexBop++;
                            obj.Bop = listBop[indexBop];
                            obj.BopId = listBop[indexBop].Id;
                        }
                        else { indexBop = 0; }
                        if (indexBop == startIndexBop) { return null; }
                    }
                }
            }

            return obj;
        }

        public async Task<BopTrackCar?> GetTemp() { return await SetNextAvailable(new BopTrackCar()); }

        public async Task<bool> HasChildObjects(int id)
        {
            return false;
        }
    }
}
