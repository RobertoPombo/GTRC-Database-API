using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class UserDatetimeService(IUserDatetimeContext iUserDatetimeContext,
        IBaseContext<User> iUserContext,
        IBaseContext<UserDatetime> iBaseContext) : BaseService<UserDatetime>(iBaseContext)
    {
        public bool Validate(UserDatetime? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            User? user = null;
            if (obj.User is not null) { user = iUserContext.GetById(obj.UserId).Result; };
            if (user is null)
            {
                List<User> list = iUserContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.User = list[0]; obj.UserId = list[0].Id; isValid = false; }
            }
            else { obj.User = user; }
            if (obj.Date > GlobalValues.DateTimeMaxValue) { obj.Date = GlobalValues.DateTimeMaxValue; isValid = false; }
            else if (obj.Date < GlobalValues.DateTimeMinValue) { obj.Date = GlobalValues.DateTimeMinValue; isValid = false; }
            if (obj.EloRating > User.MaxEloRating) { obj.EloRating = User.MaxEloRating; isValid = false; }
            else if (obj.EloRating < User.MinEloRating) { obj.EloRating = User.MinEloRating; isValid = false; }
            if (obj.SafetyRating > User.MaxSafetyRating) { obj.SafetyRating = User.MaxSafetyRating; isValid = false; }

            return isValid;
        }

        public async Task<bool> SetNextAvailable(UserDatetime? obj)
        {
            bool isAvailable = true;
            if (obj is null) { return false; }

            DateTime startDate = obj.Date;
            while (!await IsUnique(obj))
            {
                isAvailable = false;
                if (obj.Date < GlobalValues.DateTimeMaxValue.AddDays(-1)) { obj.Date = obj.Date.AddDays(1); } else { obj.Date = GlobalValues.DateTimeMinValue; }
                if (obj.Date == startDate)
                {
                    int startIndexUser = 0;
                    List<int> idListUser = [];
                    List<User> listUser = iUserContext.GetAll().Result;
                    for (int index = 0; index < listUser.Count; index++)
                    {
                        idListUser.Add(listUser[index].Id);
                        if (listUser[index].Id == obj.UserId) { startIndexUser = index; }
                    }
                    int indexUser = startIndexUser;

                    if (indexUser < idListUser.Count - 1)
                    {
                        indexUser++;
                        obj.User = listUser[indexUser];
                        obj.UserId = listUser[indexUser].Id;
                    }
                    else { indexUser = 0; }
                    if (indexUser == startIndexUser) { obj = null; return false; }
                }
            }

            return isAvailable;
        }

        public async Task<UserDatetime?> GetTemp() { UserDatetime obj = new(); Validate(obj); await SetNextAvailable(obj); return obj; }
    }
}
