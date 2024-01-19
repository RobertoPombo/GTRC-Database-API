using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Community))]
    public class CommunityController(CommunityService service, BaseService<Community> baseService) : BaseController<Community>(baseService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Community?>> GetByUniqProps(CommunityUniqPropsDto0 objDto)
        {
            UniqPropsDto<Community> _objDto = new() { Index = 0, Dto = objDto };
            Community? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Community>>> GetByProps(CommunityAddDto objDto)
        {
            AddDto<Community> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Community>>> GetByFilter(CommunityFilterDtos objDto)
        {
            FilterDtos<Community> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Community?>> GetTemp()
        {
            Community? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Community?>> Add(CommunityAddDto objDto)
        {
            Community? objValidated = service.Validate(objDto.Dto2Model());
            Community? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null || objValidated is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
            else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Community> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Community?>> Update(CommunityUpdateDto objDto)
        {
            Community? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                Community? objValidated = service.Validate(objDto.Dto2Model(obj));
                obj = await service.SetNextAvailable(objDto.Dto2Model(obj));
                if (obj is null || objValidated is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
                else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }

        [HttpDelete("Delete/{id}/{force}")] public async Task<ActionResult> Delete(int id, bool force = false)
        {
            Community? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await service.HasChildObjects(obj.Id)) { return StatusCode(405); }
            else { await service.Delete(obj); return Ok(); }
        }
    }
}
