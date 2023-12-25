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
        [HttpGet("Get/ByUniqProps")]
        public async Task<ActionResult<Series?>> GetByUniqProps([FromQuery] SeriesUniqPropsDto0 objDto)
        {
            Series? obj = await service.GetByUniqProps(objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/By")]
        public async Task<ActionResult<List<Series>>> GetByProps([FromQuery] SeriesAddDto objDto)
        {
            return Ok(await service.GetByProps(objDto));
        }

        [HttpGet("Get/ByFilter")]
        public async Task<ActionResult<List<Series>>> GetByFilter([FromQuery] SeriesFilterDtos objDto)
        {
            return Ok(await service.GetByFilter(objDto.Filter, objDto.FilterMin, objDto.FilterMax));
        }

        [HttpGet("Get/Temp")]
        public async Task<ActionResult<Series?>> GetTemp()
        {
            Series? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Series?>> Add(SeriesAddDto objDto)
        {
            Series? obj = await service.SetNextAvailable(SeriesService.Validate(objDto.Map()));
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
            else { await service.Add(obj); SeriesUniqPropsDto0 objDto0 = new(); objDto0.ReMap(obj); return Ok(await service.GetByUniqProps(objDto0)); }
        }

        [HttpPut("Update")]
        public async Task<ActionResult<Series?>> Update(SeriesUpdateDto objDto)
        {
            Series? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = await service.SetNextAvailable(SeriesService.Validate(objDto.Map(obj)));
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }
    }
}
