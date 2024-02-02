using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(OrganizationUser))]
    public class OrganizationUserController(OrganizationUserService service, BaseService<OrganizationUser> baseService, FullService<OrganizationUser> fullService) : BaseController<OrganizationUser>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<OrganizationUser?>> GetByUniqProps(OrganizationUserUniqPropsDto0 objDto)
        {
            UniqPropsDto<OrganizationUser> _objDto = new() { Index = 0, Dto = objDto };
            OrganizationUser? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<OrganizationUser>>> GetByProps(OrganizationUserAddDto objDto)
        {
            AddDto<OrganizationUser> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<OrganizationUser>>> GetByFilter(OrganizationUserFilterDtos objDto)
        {
            FilterDtos<OrganizationUser> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<OrganizationUser?>> GetTemp()
        {
            OrganizationUser? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<OrganizationUser?>> Add(OrganizationUserAddDto objDto)
        {
            OrganizationUser? objValidated = service.Validate(objDto.Dto2Model());
            OrganizationUser? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null || objValidated is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
            else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<OrganizationUser> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<OrganizationUser?>> Update(OrganizationUserUpdateDto objDto)
        {
            OrganizationUser? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                OrganizationUser? objValidated = service.Validate(objDto.Dto2Model(obj));
                obj = await service.SetNextAvailable(objDto.Dto2Model(obj));
                if (obj is null || objValidated is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
                else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }

        [HttpGet("Get/Admins/{organizationId}")] public ActionResult<OrganizationUser?> GetAdmins(int organizationId)
        {
            return Ok(service.GetAdmins(organizationId));
        }

        [HttpDelete("Delete/{id}/{force}", Order = -1)] public new async Task<ActionResult> Delete(int id, bool force = false)
        {
            OrganizationUser? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await fullService.HasChildObjects(obj.Id, true)) { return StatusCode(405); }
            else if (service.IsOnlyAdmin(obj)) { return StatusCode(405); }
            else { await service.Delete(obj); return Ok(); }
        }
    }
}
