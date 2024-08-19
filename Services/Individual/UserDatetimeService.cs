using System.Net;

using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;
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

            if (obj.EloRating > UserDatetime.MaxEloRating) { obj.EloRating = UserDatetime.MaxEloRating; isValid = false; }
            else if (obj.EloRating < UserDatetime.MinEloRating) { obj.EloRating = UserDatetime.MinEloRating; isValid = false; }
            if (obj.SafetyRating > UserDatetime.MaxSafetyRating) { obj.SafetyRating = UserDatetime.MaxSafetyRating; isValid = false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(UserDatetime? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            User? user = null;
            if (obj.User is not null) { user = iUserContext.GetById(obj.UserId).Result; };
            if (user is null)
            {
                List<User> list = iUserContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.User = list[0]; obj.UserId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.User = user; }
            if (obj.Date > GlobalValues.DateTimeMaxValue) { obj.Date = GlobalValues.DateTimeMaxValue; isValidUniqProps = false; }
            else if (obj.Date < GlobalValues.DateTimeMinValue) { obj.Date = GlobalValues.DateTimeMinValue; isValidUniqProps = false; }

            int startIndexUser = 0;
            List<int> idListUser = [];
            List<User> listUser = iUserContext.GetAll().Result;
            for (int index = 0; index < listUser.Count; index++)
            {
                idListUser.Add(listUser[index].Id);
                if (listUser[index].Id == obj.UserId) { startIndexUser = index; }
            }
            int indexUser = startIndexUser;

            DateTime startDate = obj.Date;
            while (!await IsUnique(obj))
            {
                isValidUniqProps = false;
                if (obj.Date < GlobalValues.DateTimeMaxValue.AddDays(-1)) { obj.Date = obj.Date.AddDays(1); } else { obj.Date = GlobalValues.DateTimeMinValue; }
                if (obj.Date == startDate)
                {
                    if (indexUser < idListUser.Count - 1) { indexUser++; }
                    else { indexUser = 0; }
                    obj.User = listUser[indexUser];
                    obj.UserId = listUser[indexUser].Id;
                    if (indexUser == startIndexUser) { obj = null; return false; }
                }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<UserDatetime?> GetTemp() { UserDatetime obj = new(); await ValidateUniqProps(obj); return obj; }

        public async Task<(HttpStatusCode, UserDatetime?)> GetAnyByUniqProps(UserDatetimeUniqPropsDto0 objDto)
        {
            UserDatetimeAddDto _objDtoDto = Scripts.Map(objDto, new UserDatetimeAddDto());
            _objDtoDto.Date = null;
            AddDto<UserDatetime> _objDto = new() { Dto = _objDtoDto };
            List<UserDatetime> list = Scripts.SortByDate(await GetByProps(_objDto));
            if (list.Count == 0 || list[0].Date > objDto.Date)
            {
                User? user = await iUserContext.GetById(objDto.UserId);
                if (user is null) { return (HttpStatusCode.NotFound, null); }
                else
                {
                    UserDatetime newObj = new() { UserId = user.Id, Date = user.RegisterDate };
                    await ValidateUniqProps(newObj);
                    if (newObj is not null) { return (HttpStatusCode.OK, newObj); }
                    else { return (HttpStatusCode.NotAcceptable, newObj); }
                }
            }
            else
            {
                for (int index = 0; index < list.Count - 1; index++) { if (list[index + 1].Date > objDto.Date) { return (HttpStatusCode.OK, list[index]); } }
                return (HttpStatusCode.OK, list[^1]);
            }
        }
    }
}
