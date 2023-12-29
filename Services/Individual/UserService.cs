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

        public static string GetFullName(string firstName, string lastName) // in FullDto verschieben und bei Http Anfragen mit zurückgeben
        {
            return firstName + " " + lastName;
        }

        public static string GetShortName(string firstName, string lastName) // in FullDto verschieben und bei Http Anfragen mit zurückgeben
        {
            string _shortName = string.Empty;
            List<char> cList = [' ', '-'];
            if (firstName is not null)
            {
                for (int index = 0; index < firstName.Length - 1; index++)
                {
                    if (_shortName.Length == 0 && !cList.Contains(firstName[index]))
                    {
                        _shortName = firstName[index].ToString() + ".";
                    }
                    else if (cList.Contains(firstName[index]) && !cList.Contains(firstName[index + 1]))
                    {
                        _shortName += firstName[index].ToString() + firstName[index + 1].ToString() + ".";
                    }
                }
                _shortName += " " + lastName;
            }
            else { _shortName = lastName; }
            return _shortName;
        }
    }
}
