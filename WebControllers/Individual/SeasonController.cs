using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Season))]
    public class SeasonController(SeasonService service, BaseService<Season> baseService, FullService<Season> fullService) : BaseController<Season>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Season?>> GetByUniqProps(SeasonUniqPropsDto0 objDto)
        {
            UniqPropsDto<Season> _objDto = new() { Index = 0, Dto = objDto };
            Season? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Season>>> GetByProps(SeasonAddDto objDto)
        {
            AddDto<Season> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Season>>> GetByFilter(SeasonFilterDtos objDto)
        {
            FilterDtos<Season> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Season?>> GetTemp()
        {
            Season? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Season?>> Add(SeasonAddDto objDto)
        {
            Season? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Season> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Season?>> Update(SeasonUpdateDto objDto)
        {
            Season? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(Season), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpGet("Get/Current/{seriesId}")] public async Task<ActionResult<Season?>> GetCurrent(int seriesId, DateTime date)
        {
            Season? obj = await service.GetCurrent(seriesId, date);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }
    }
}
