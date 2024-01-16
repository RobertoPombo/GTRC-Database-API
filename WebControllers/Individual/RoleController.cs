using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Role))]
    public class RoleController(RoleService service, BaseService<Role> baseService) : BaseController<Role>(baseService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Role?>> GetByUniqProps(RoleUniqPropsDto0 objDto)
        {
            UniqPropsDto<Role> _objDto = new() { Index = 0, Dto = objDto };
            Role? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Role>>> GetByProps(RoleAddDto objDto)
        {
            AddDto<Role> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Role>>> GetByFilter(RoleFilterDtos objDto)
        {
            FilterDtos<Role> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Role?>> GetTemp()
        {
            Role? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Role?>> Add(RoleAddDto objDto)
        {
            Role? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Role> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Role?>> Update(RoleUpdateDto objDto)
        {
            Role? obj = await service.GetById(objDto.Id);
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
            Role? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await service.HasChildObjects(obj.Id)) { return Unauthorized(); }
            else { await service.Delete(obj); return Ok(); }
        }
    }
}
