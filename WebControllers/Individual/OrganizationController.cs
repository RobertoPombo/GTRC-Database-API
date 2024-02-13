using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;
using GTRC_Basics;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Organization))]
    public class OrganizationController(OrganizationService service, BaseService<Organization> baseService, FullService<Organization> fullService) : BaseController<Organization>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Organization?>> GetByUniqProps(OrganizationUniqPropsDto0 objDto)
        {
            UniqPropsDto<Organization> _objDto = new() { Index = 0, Dto = objDto };
            Organization? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Organization>>> GetByProps(OrganizationAddDto objDto)
        {
            AddDto<Organization> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Organization>>> GetByFilter(OrganizationFilterDtos objDto)
        {
            FilterDtos<Organization> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Organization?>> GetTemp()
        {
            Organization? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Organization?>> Add(OrganizationAddDto objDto)
        {
            Organization? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isAvailable = await service.SetNextAvailable(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isAvailable) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Organization> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Organization?>> Update(OrganizationUpdateDto objDto)
        {
            Organization? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = objDto.Dto2Model(obj);
                bool isValid = service.Validate(obj);
                bool isAvailable = await service.SetNextAvailable(obj);
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!isAvailable) { return StatusCode(208, await service.SetNextAvailable(obj)); }
                else if (!isValid) { return StatusCode(406, obj); }
                else
                {
                    await service.Update(obj);
                    await fullService.UpdateChildObjects(typeof(Organization), obj);
                    return Ok(obj);
                }
            }
        }
    }
}
