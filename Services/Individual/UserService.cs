using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class UserService(IUserContext iUserContext,
        IBaseContext<User> iBaseContext) : BaseService<User>(iBaseContext)
    {
        public bool Validate(User? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            if (!Scripts.IsValidSteamId(obj.SteamId) && obj.SteamId != GlobalValues.NoSteamId) { obj.SteamId = GlobalValues.NoSteamId; isValid = false; }
            if (!Scripts.IsValidDiscordId(obj.DiscordId) && obj.DiscordId != GlobalValues.NoDiscordId) { obj.DiscordId = GlobalValues.NoDiscordId; isValid = false; }
            obj.FirstName = Scripts.RemoveSpaceStartEnd(obj.FirstName);
            if (obj.FirstName == string.Empty) { obj.FirstName = nameof(obj.FirstName); isValid = false; }
            obj.LastName = Scripts.RemoveSpaceStartEnd(obj.LastName);
            if (obj.LastName == string.Empty) { obj.LastName = nameof(obj.LastName); isValid = false; }
            if (obj.RegisterDate > DateTime.UtcNow || obj.RegisterDate < GlobalValues.DateTimeMinValue) { obj.RegisterDate = DateTime.UtcNow; isValid = false; }
            if (obj.BanDate < obj.RegisterDate) { obj.BanDate = obj.RegisterDate; isValid = false; }
            if (obj.Name3Digits.Length == 3) { obj.Name3Digits = obj.Name3Digits.ToUpper(); } else { obj.Name3Digits = string.Empty; }
            if (obj.EloRating > User.MaxEloRating) { obj.EloRating = User.MaxEloRating; isValid = false; }
            else if (obj.EloRating < User.MinEloRating) { obj.EloRating = User.MinEloRating; isValid = false; }
            if (obj.SafetyRating > User.MaxSafetyRating) { obj.SafetyRating = User.MaxSafetyRating; isValid = false; }
            obj.NickName = Scripts.RemoveSpaceStartEnd(obj.NickName);
            if (obj.NickName == string.Empty) { obj.NickName = nameof(obj.NickName); isValid = false; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(User? obj)
        {
            bool isAvailable = true;
            if (obj is null) { return false; }

            ulong? startValue = obj.SteamId;
            while (!await IsUnique(obj, 0) && obj.SteamId != GlobalValues.NoSteamId)
            {
                isAvailable = false;
                if (obj.SteamId < GlobalValues.MaxSteamId) { obj.SteamId++; } else { obj.SteamId = GlobalValues.MinSteamId; }
                if (obj.SteamId == startValue) { obj.SteamId = GlobalValues.NoSteamId; }
            }

            startValue = obj.DiscordId;
            while (!await IsUnique(obj, 1) && obj.DiscordId != GlobalValues.NoDiscordId)
            {
                isAvailable = false;
                if (obj.DiscordId < GlobalValues.MaxDiscordId) { obj.DiscordId++; } else { obj.DiscordId = GlobalValues.MinDiscordId; }
                if (obj.DiscordId == startValue) { obj.DiscordId = GlobalValues.NoDiscordId; }
            }

            return isAvailable;
        }

        public async Task<User?> GetTemp() { User obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }

        public List<string> GetName3DigitsOptions(User obj)
        {
            List<string> listFirstNames; List<string> listLastNames;
            List<string> tempListN3D = [];
            listFirstNames = FilterLetters4N3D(obj.FirstName);
            listLastNames = FilterLetters4N3D(obj.LastName);
            List<string> listAllNames = [.. listFirstNames, .. listLastNames];
            tempListN3D = AddN3D(tempListN3D, obj.Name3Digits ?? "");
            string n3D = "";
            foreach (string _name in listLastNames) { n3D += _name[0]; }
            tempListN3D = AddN3D(tempListN3D, n3D);
            n3D = "";
            foreach (string _name in listAllNames) { n3D += _name[0]; }
            tempListN3D = AddN3D(tempListN3D, n3D);
            foreach (string _name in listLastNames) { tempListN3D = AddN3D(tempListN3D, _name); }
            n3D = "";
            foreach (string _name in listLastNames) { n3D += _name; }
            tempListN3D = AddN3D(tempListN3D, n3D);
            foreach (string _fname in listFirstNames)
            {
                n3D = _fname[..1];
                foreach (string _name in listLastNames) { n3D += _name; }
                tempListN3D = AddN3D(tempListN3D, n3D);
            }
            n3D = "";
            foreach (string _name in listLastNames)
            {
                n3D += _name[..1] + Scripts.StrRemoveVocals(_name[1..]);
            }
            tempListN3D = AddN3D(tempListN3D, n3D);
            foreach (string _fname in listFirstNames)
            {
                n3D = _fname[..1];
                foreach (string _name in listLastNames)
                {
                    n3D += _name[..1] + Scripts.StrRemoveVocals(_name[1..]);
                }
                tempListN3D = AddN3D(tempListN3D, n3D);
            }
            foreach (string _fname in listFirstNames) { tempListN3D = AddN3D(tempListN3D, _fname); }
            n3D = "";
            foreach (string _name in listLastNames)
            {
                n3D += _name[..1] + Scripts.StrRemoveVocals(_name[1..]);
            }
            if (n3D.Length > 2)
            {
                for (int charNr1 = 1; charNr1 < n3D.Length - 1; charNr1++)
                {
                    for (int charNr2 = charNr1 + 1; charNr2 < n3D.Length; charNr2++)
                    {
                        tempListN3D = AddN3D(tempListN3D, n3D[..1] + n3D[charNr1] + n3D[charNr2]);
                    }
                }
            }
            foreach (string _fname in listFirstNames)
            {
                n3D = _fname[..1];
                foreach (string _name in listLastNames)
                {
                    n3D += _name[..1] + Scripts.StrRemoveVocals(_name[1..]);
                }
                if (n3D.Length > 2)
                {
                    for (int charNr = 2; charNr < n3D.Length; charNr++)
                    {
                        tempListN3D = AddN3D(tempListN3D, n3D[..2] + n3D[charNr]);
                    }
                }
            }
            n3D = "";
            foreach (string _name in listLastNames) { n3D += Scripts.StrRemoveVocals(_name[1..]); }
            if (n3D.Length > 2)
            {
                for (int charNr1 = 1; charNr1 < n3D.Length - 1; charNr1++)
                {
                    for (int charNr2 = charNr1 + 1; charNr2 < n3D.Length; charNr2++)
                    {
                        tempListN3D = AddN3D(tempListN3D, n3D[..1] + n3D[..1] + n3D[charNr2]);
                    }
                }
            }
            foreach (string _fname in listFirstNames)
            {
                n3D = _fname[..1];
                foreach (string _name in listLastNames) { n3D += _name; }
                if (n3D.Length > 2)
                {
                    for (int charNr = 2; charNr < n3D.Length; charNr++)
                    {
                        tempListN3D = AddN3D(tempListN3D, n3D[..2] + n3D[charNr]);
                    }
                }
            }
            n3D = "";
            foreach (string _name in listAllNames) { n3D += _name; }
            n3D += "XXX";
            return AddN3D(tempListN3D, n3D);
        }

        public static List<string> AddN3D(List<string> tempListN3D, string n3D)
        {
            if (n3D.Length > 2 && !tempListN3D.Contains(n3D[..3])) { tempListN3D.Add(n3D[..3]); }
            return tempListN3D;
        }

        public static List<string> FilterLetters4N3D(string name)
        {
            name = Scripts.StrRemoveSpecialLetters(name);
            name = name.ToUpper();
            name = name.Replace("-", " ");
            List<string> nameList = [];
            foreach (string _name in name.Split(' ')) { if (_name.Length > 0) { nameList.Add(_name); } }
            return nameList;
        }
    }
}
