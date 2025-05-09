﻿using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class StintanalysismethodService(IStintanalysismethodContext iStintanalysismethodContext,
        IBaseContext<Stintanalysismethod> iBaseContext) : BaseService<Stintanalysismethod>(iBaseContext)
    {
        public bool Validate(Stintanalysismethod? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (obj.MaxTimeDeltaPercent < Stintanalysismethod.MinMaxTimeDeltaPercent) { obj.MaxTimeDeltaPercent = Stintanalysismethod.MinMaxTimeDeltaPercent; isValid = false; }
            if (obj.MinLapsCount > obj.LapRange) { obj.MinLapsCount = obj.LapRange; isValid = false; }
            if (obj.MinLapsCount > obj.MaxLapsCount) { obj.MinLapsCount = obj.MaxLapsCount; isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Stintanalysismethod? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Stintanalysismethod.DefaultName; isValidUniqProps = false; }

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

        public async Task<Stintanalysismethod?> GetTemp() { Stintanalysismethod obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
