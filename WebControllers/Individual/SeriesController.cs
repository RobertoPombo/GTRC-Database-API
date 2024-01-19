using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Series))]
    public class SeriesController(SeriesService service, BaseService<Series> baseService) : BaseController<Series>(baseService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Series?>> GetByUniqProps(SeriesUniqPropsDto0 objDto)
        {
            UniqPropsDto<Series> _objDto = new() { Index = 0, Dto = objDto };
            Series? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Series>>> GetByProps(SeriesAddDto objDto)
        {
            AddDto<Series> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Series>>> GetByFilter(SeriesFilterDtos objDto)
        {
            FilterDtos<Series> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Series?>> GetTemp()
        {
            Series? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Series?>> Add(SeriesAddDto objDto)
        {
            Series? objValidated = service.Validate(objDto.Dto2Model());
            Series? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null || objValidated is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
            else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Series> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Series?>> Update(SeriesUpdateDto objDto)
        {
            Series? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                Series? objValidated = service.Validate(objDto.Dto2Model(obj));
                obj = await service.SetNextAvailable(objDto.Dto2Model(obj));
                if (obj is null || objValidated is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
                else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }

        [HttpDelete("Delete/{id}/{force}")] public async Task<ActionResult> Delete(int id, bool force = false)
        {
            Series? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await service.HasChildObjects(obj.Id)) { return StatusCode(405); }
            else { await service.Delete(obj); return Ok(); }
        }
    }
}
