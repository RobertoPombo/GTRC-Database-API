using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class UserService(IUserContext iUserContext, IBaseContext<User> iBaseContext) : BaseService<User>(iBaseContext)
    {
        public static User Validate(User obj)
        {
            if (!IsValidSteamId(obj.SteamId)) { obj.SteamId = User.MinSteamId; }
            if (!IsValidDiscordId(obj.DiscordId)) { obj.DiscordId = User.NoDiscordId; }
            obj.FirstName = Scripts.RemoveSpaceStartEnd(obj.FirstName);
            obj.LastName = Scripts.RemoveSpaceStartEnd(obj.LastName);
            if (obj.RegisterDate > DateTime.UtcNow || obj.RegisterDate < GlobalValues.DateTimeMinValue) { obj.RegisterDate = DateTime.UtcNow; }
            if (obj.BanDate < obj.RegisterDate) { obj.BanDate = obj.RegisterDate; }
            if (obj.Name3Digits.Length == 3) { obj.Name3Digits = obj.Name3Digits.ToUpper(); } else { obj.Name3Digits = string.Empty; }
            obj.EloRating = Math.Min(Math.Max(obj.EloRating, User.MinEloRating), User.MaxEloRating);
            obj.SafetyRating = Math.Min(obj.SafetyRating, User.MaxSafetyRating);
            return obj;
        }

        public async Task<User?> SetNextAvailable(User obj)
        {
            ulong startValue = obj.SteamId;
            while (!await IsUnique(obj, 0))
            {
                if (obj.SteamId < User.MaxSteamId) { obj.SteamId++; } else { obj.SteamId = User.MinSteamId; }
                if (obj.SteamId == startValue) { return null; }
            }
            startValue = obj.DiscordId;
            while (!await IsUnique(obj, 1) && obj.DiscordId != User.NoDiscordId)
            {
                if (obj.DiscordId < User.MaxDiscordId) { obj.DiscordId++; } else { obj.DiscordId = User.MinDiscordId; }
                if (obj.DiscordId == startValue) { return null; }
            }
            return obj;
        }

        public async Task<User?> GetTemp() { return await SetNextAvailable(Validate(new User())); }

        public static bool IsValidSteamId(ulong steamId)
        {
            return steamId >= User.MinSteamId && steamId <= User.MaxSteamId;
        }

        public static bool IsValidDiscordId(ulong discordId)
        {
            return discordId >= User.MinDiscordId && discordId <= User.MaxDiscordId;
        }

        public static ulong? String2LongSteamID(string? strSteamId)
        {
            ulong steamId = ulong.MinValue;
            strSteamId = new string(strSteamId?.Where(Char.IsNumber).ToArray());
            _ = ulong.TryParse(strSteamId, out steamId);
            if (IsValidSteamId(steamId)) { return steamId; }
            else { return null; }
        }

        public List<string> GetName3DigitsOptions(User user)
        {
            List<string> listFirstNames; List<string> listLastNames;
            List<string> tempListN3D = [];
            listFirstNames = FilterLetters4N3D(user.FirstName);
            listLastNames = FilterLetters4N3D(user.LastName);
            List<string> listAllNames = [.. listFirstNames, .. listLastNames];
            tempListN3D = AddN3D(tempListN3D, user.Name3Digits);
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
