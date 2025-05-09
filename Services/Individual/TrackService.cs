﻿using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class TrackService(ITrackContext iTrackContext,
        IBaseContext<Track> iBaseContext) : BaseService<Track>(iBaseContext)
    {
        public bool Validate(Track? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(Track? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            obj.Name = Scripts.RemoveSpaceStartEnd(obj.Name);
            if (obj.Name == string.Empty) { obj.Name = Track.DefaultName; isValidUniqProps = false; }
            obj.AccTrackId = Scripts.RemoveSpaceStartEnd(obj.AccTrackId).Replace(" ", "_");
            if (obj.AccTrackId == string.Empty) { obj.AccTrackId = Track.DefaultAccTrackId; isValidUniqProps = false; }
            obj.NameLfm = Scripts.RemoveSpaceStartEnd(obj.NameLfm);
            obj.NameGoogleSheets = Scripts.RemoveSpaceStartEnd(obj.NameGoogleSheets);

            int nr = 1;
            string delimiter = " #";
            string defName = obj.Name;
            string[] defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 0))
            {
                isValidUniqProps = false;
                obj.Name = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            nr = 1;
            delimiter = "_";
            defName = obj.AccTrackId;
            defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 1))
            {
                isValidUniqProps = false;
                obj.AccTrackId = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj = null; return false; }
            }

            nr = 1;
            delimiter = " #";
            defName = obj.NameLfm;
            defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 2) && obj.NameLfm != string.Empty)
            {
                isValidUniqProps = false;
                obj.NameLfm = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj.NameLfm = string.Empty; }
            }

            nr = 1;
            delimiter = " #";
            defName = obj.NameGoogleSheets;
            defNameList = defName.Split(delimiter);
            if (defNameList.Length > 1 && int.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
            while (!await IsUnique(obj, 3) && obj.NameGoogleSheets != string.Empty)
            {
                isValidUniqProps = false;
                obj.NameGoogleSheets = defName + delimiter + nr.ToString();
                nr++;
                if (nr == int.MaxValue) { obj.NameGoogleSheets = string.Empty; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<Track?> GetTemp() { Track obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
