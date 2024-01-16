using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(UserRole))]
    public class UserRoleController(UserRoleService service, BaseService<UserRole> baseService) : BaseController<UserRole>(baseService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<UserRole?>> GetByUniqProps(UserRoleUniqPropsDto0 objDto)
        {
            UniqPropsDto<UserRole> _objDto = new() { Index = 0, Dto = objDto };
            UserRole? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<UserRole>>> GetByProps(UserRoleAddDto objDto)
        {
            AddDto<UserRole> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<UserRole>>> GetByFilter(UserRoleFilterDtos objDto)
        {
            FilterDtos<UserRole> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<UserRole?>> GetTemp()
        {
            UserRole? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<UserRole?>> Add(UserRoleAddDto objDto)
        {
            UserRole? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<UserRole> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<UserRole?>> Update(UserRoleUpdateDto objDto)
        {
            UserRole? obj = await service.GetById(objDto.Id);
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
            UserRole? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await service.HasChildObjects(obj.Id)) { return Unauthorized(); }
            else { await service.Delete(obj); return Ok(); }
        }
    }
}
