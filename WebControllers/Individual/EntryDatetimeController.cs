using Microsoft.AspNetCore.Mvc;
using System.Net;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(EntryDatetime))]
    public class EntryDatetimeController(EntryDatetimeService service, BaseService<EntryDatetime> baseService, FullService<EntryDatetime> fullService) : BaseController<EntryDatetime>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<EntryDatetime?>> GetByUniqProps(EntryDatetimeUniqPropsDto0 objDto)
        {
            UniqPropsDto<EntryDatetime> _objDto = new() { Index = 0, Dto = objDto };
            EntryDatetime? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<EntryDatetime>>> GetByProps(EntryDatetimeAddDto objDto)
        {
            AddDto<EntryDatetime> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<EntryDatetime>>> GetByFilter(EntryDatetimeFilterDtos objDto)
        {
            FilterDtos<EntryDatetime> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<EntryDatetime?>> GetTemp()
        {
            EntryDatetime? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<EntryDatetime?>> Add(EntryDatetimeAddDto objDto)
        {
            EntryDatetime? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<EntryDatetime> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<EntryDatetime?>> Update(EntryDatetimeUpdateDto objDto)
        {
            EntryDatetime? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = objDto.Dto2Model(obj);
                bool isValid = service.Validate(obj);
                bool isValidUniqProps = await service.ValidateUniqProps(obj);
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!isValidUniqProps) { return StatusCode(208, obj); }
                else if (!isValid) { return StatusCode(406, obj); }
                else
                {
                    await service.Update(obj);
                    await fullService.UpdateChildObjects(typeof(EntryDatetime), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpPut("Get/ByUniqProps/0/Any")] public async Task<ActionResult<EntryDatetime?>> GetAnyByUniqProps(EntryDatetimeUniqPropsDto0 objDto)
        {
            (HttpStatusCode status, EntryDatetime? obj) = await service.GetAnyByUniqProps(objDto);
            if (status == HttpStatusCode.OK) { return Ok(obj); }
            else if (status == HttpStatusCode.NotAcceptable) { return StatusCode(406, obj); }
            else if (status == HttpStatusCode.NotFound) { return NotFound(obj); }
            else { return StatusCode(500, obj); }
        }
    }
}
