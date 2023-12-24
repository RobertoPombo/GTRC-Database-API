using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Database_API.Services.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(User))]
    public class UserController(UserService service, BaseService<User> baseService) : BaseController<User>(baseService)
    {
        [HttpGet("Get/ByUniqProps")]
        public async Task<ActionResult<User?>> GetByUniqProps([FromQuery] UserUniqPropsDto0 objDto)
        {
            User? obj = await service.GetByUniqProps(objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/By")]
        public async Task<ActionResult<List<User>>> GetByProps([FromQuery] UserAddDto objDto)
        {
            return Ok(await service.GetByProps(objDto));
        }

        [HttpGet("Get/ByFilter")]
        public async Task<ActionResult<List<User>>> GetByFilter([FromQuery] UserFilterDtos objDto)
        {
            return Ok(await service.GetByFilter(objDto.Filter, objDto.FilterMin, objDto.FilterMax));
        }

        [HttpGet("Get/Temp")]
        public async Task<ActionResult<User?>> GetTemp()
        {
            User? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")]
        public async Task<ActionResult<User?>> Add(UserAddDto objDto)
        {
            User? obj = await service.SetNextAvailable(UserService.Validate(objDto.Map()));
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
            else { await service.Add(obj); UserUniqPropsDto0 objDto0 = new(); objDto0.ReMap(obj); return Ok(await service.GetByUniqProps(objDto0)); }
        }

        [HttpPut("Update")]
        public async Task<ActionResult<User?>> Update(UserUpdateDto objDto)
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
