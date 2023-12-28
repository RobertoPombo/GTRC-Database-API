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
        [HttpGet("Get/ByUniqProps/0")] public async Task<ActionResult<User?>> GetByUniqProps([FromQuery] UserUniqPropsDto0 objDto)
        {
            UniqPropsDto<User> _objDto = new() { Index = 0, Dto = objDto };
            User? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/ByUniqProps/1")] public async Task<ActionResult<User?>> GetByUniqProps([FromQuery] UserUniqPropsDto1 objDto)
        {
            UniqPropsDto<User> _objDto = new() { Index = 1, Dto = objDto };
            User? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/ByProps")] public async Task<ActionResult<List<User>>> GetByProps([FromQuery] UserAddDto objDto)
        {
            AddDto<User> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpGet("Get/ByFilter")] public async Task<ActionResult<List<User>>> GetByFilter([FromQuery] UserFilterDtos objDto)
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
            User? obj = await service.SetNextAvailable(UserService.Validate(objDto.Map()));
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
            else { await service.Add(obj); UniqPropsDto<User> uniqPropsDto = new(); uniqPropsDto.Dto.ReMap(obj); return Ok(await service.GetByUniqProps(uniqPropsDto)); }
        }

        [HttpPut("Update")] public async Task<ActionResult<User?>> Update(UserUpdateDto objDto)
        {
            User? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = await service.SetNextAvailable(UserService.Validate(objDto.Map(obj)));
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }
    }
}
