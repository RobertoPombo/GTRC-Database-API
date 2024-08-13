using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(SeriesDiscordchanneltype))]
    public class SeriesDiscordchanneltypeController(SeriesDiscordchanneltypeService service, BaseService<SeriesDiscordchanneltype> baseService, FullService<SeriesDiscordchanneltype> fullService) : BaseController<SeriesDiscordchanneltype>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<SeriesDiscordchanneltype?>> GetByUniqProps(SeriesDiscordchanneltypeUniqPropsDto0 objDto)
        {
            UniqPropsDto<SeriesDiscordchanneltype> _objDto = new() { Index = 0, Dto = objDto };
            SeriesDiscordchanneltype? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByUniqProps/1")] public async Task<ActionResult<SeriesDiscordchanneltype?>> GetByUniqProps(SeriesDiscordchanneltypeUniqPropsDto1 objDto)
        {
            UniqPropsDto<SeriesDiscordchanneltype> _objDto = new() { Index = 1, Dto = objDto };
            SeriesDiscordchanneltype? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<SeriesDiscordchanneltype>>> GetByProps(SeriesDiscordchanneltypeAddDto objDto)
        {
            AddDto<SeriesDiscordchanneltype> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<SeriesDiscordchanneltype>>> GetByFilter(SeriesDiscordchanneltypeFilterDtos objDto)
        {
            FilterDtos<SeriesDiscordchanneltype> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<SeriesDiscordchanneltype?>> GetTemp()
        {
            SeriesDiscordchanneltype? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<SeriesDiscordchanneltype?>> Add(SeriesDiscordchanneltypeAddDto objDto)
        {
            SeriesDiscordchanneltype? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<SeriesDiscordchanneltype> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<SeriesDiscordchanneltype?>> Update(SeriesDiscordchanneltypeUpdateDto objDto)
        {
            SeriesDiscordchanneltype? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(SeriesDiscordchanneltype), obj);
                    return Ok(obj);
                }
            }
        }
    }
}
