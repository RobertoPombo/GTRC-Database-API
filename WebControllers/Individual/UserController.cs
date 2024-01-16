using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(User))]
    public class UserController(UserService service, BaseService<User> baseService) : BaseController<User>(baseService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<User?>> GetByUniqProps(UserUniqPropsDto0 objDto)
        {
            UniqPropsDto<User> _objDto = new() { Index = 0, Dto = objDto };
            User? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByUniqProps/1")] public async Task<ActionResult<User?>> GetByUniqProps(UserUniqPropsDto1 objDto)
        {
            UniqPropsDto<User> _objDto = new() { Index = 1, Dto = objDto };
            User? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<User>>> GetByProps(UserAddDto objDto)
        {
            AddDto<User> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<User>>> GetByFilter(UserFilterDtos objDto)
        {
            FilterDtos<User> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<User?>> GetTemp()
        {
            User? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<User?>> Add(UserAddDto objDto)
        {
            User? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<User> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<User?>> Update(UserUpdateDto objDto)
        {
            User? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = await service.SetNextAvailable(objDto.Dto2Model(obj));
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }

        [HttpDelete("Delete/{id}/{force}")] public async Task<ActionResult> Delete(int id, bool force=false)
        {
            User? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await service.HasChildObjects(obj.Id)) { return Unauthorized(); }
            else { await service.Delete(obj); return Ok(); }
        }

        [HttpGet("Get/Name3DigitsOptions/{id}")] public async Task<ActionResult<List<string>>> GetName3DigitsOptions(int id)
        {
            User? obj = await baseService.GetById(id);
            if (obj is null) { return NotFound(new List<string>()); }
            else { return Ok(service.GetName3DigitsOptions(obj)); }
        }
    }
}
