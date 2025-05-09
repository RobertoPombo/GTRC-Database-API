﻿using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class BopTrackCarService(IBopTrackCarContext iBopTrackCarContext,
        IBaseContext<Bop> iBopContext,
        IBaseContext<Track> iTrackContext,
        IBaseContext<Car> iCarContext,
        IBaseContext<BopTrackCar> iBaseContext) : BaseService<BopTrackCar>(iBaseContext)
    {
        public bool Validate(BopTrackCar? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(BopTrackCar? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Bop? bop = null;
            if (obj.Bop is not null) { bop = iBopContext.GetById(obj.BopId).Result; };
            if (bop is null)
            {
                List<Bop> list = iBopContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Bop = list[0]; obj.BopId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Bop = bop; }
            Track? track = null;
            if (obj.Track is not null) { track = iTrackContext.GetById(obj.TrackId).Result; };
            if (track is null)
            {
                List<Track> list = iTrackContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Track = list[0]; obj.TrackId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Track = track; }
            Car? car = null;
            if (obj.Car is not null) { car = iCarContext.GetById(obj.CarId).Result; };
            if (car is null)
            {
                List<Car> list = iCarContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Car = list[0]; obj.CarId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Car = car; }

            int startIndexBop = 0;
            List<int> idListBop = [];
            List<Bop> listBop = iBopContext.GetAll().Result;
            for (int index = 0; index < listBop.Count; index++)
            {
                idListBop.Add(listBop[index].Id);
                if (listBop[index].Id == obj.BopId) { startIndexBop = index; }
            }
            int indexBop = startIndexBop;

            int startIndexTrack = 0;
            List<int> idListTrack = [];
            List<Track> listTrack = iTrackContext.GetAll().Result;
            for (int index = 0; index < listTrack.Count; index++)
            {
                idListTrack.Add(listTrack[index].Id);
                if (listTrack[index].Id == obj.TrackId) { startIndexTrack = index; }
            }
            int indexTrack = startIndexTrack;

            int startIndexCar = 0;
            List<int> idListCar = [];
            List<Car> listCar = iCarContext.GetAll().Result;
            for (int index = 0; index < listCar.Count; index++)
            {
                idListCar.Add(listCar[index].Id);
                if (listCar[index].Id == obj.CarId) { startIndexCar = index; }
            }
            int indexCar = startIndexCar;

            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (indexCar < idListCar.Count - 1) { indexCar++; }
                else { indexCar = 0; }
                obj.Car = listCar[indexCar];
                obj.CarId = listCar[indexCar].Id;
                if (indexCar == startIndexCar)
                {
                    if (indexTrack < idListTrack.Count - 1) { indexTrack++; }
                    else { indexTrack = 0; }
                    obj.Track = listTrack[indexTrack];
                    obj.TrackId = listTrack[indexTrack].Id;
                    if (indexTrack == startIndexTrack)
                    {
                        if (indexBop < idListBop.Count - 1) { indexBop++; }
                        else { indexBop = 0; }
                        obj.Bop = listBop[indexBop];
                        obj.BopId = listBop[indexBop].Id;
                        if (indexBop == startIndexBop) { obj = null; return false; }
                    }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<BopTrackCar?> GetTemp() { BopTrackCar obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
