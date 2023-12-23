using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Database_API.Services.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController][Route(nameof(Track))]
    public class TrackController(TrackService service, BaseService<Track> baseService) : BaseController<Track>(baseService)
    {
        [HttpGet("Get/ByUniqProps")] public async Task<ActionResult<Track>> GetByUniqProps([FromQuery] TrackUniqPropsDto0 objDto)
        {
            Track? obj = await service.GetByUniqProps(objDto);
            if (obj is null) { return NotFound(new Track()); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/By")] public async Task<ActionResult<List<Track>>> GetByProps([FromQuery] TrackAddDto objDto)
        {
            return Ok(await service.GetByProps(objDto));
        }

        [HttpGet("Get/ByFilter")] public async Task<ActionResult<List<Track>>> GetByFilter([FromQuery] TrackFilterDtos objDto)
        {
            return Ok(await service.GetByFilter(objDto));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Track>> GetTemp()
        {
            Track? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(new Track()); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Track>> Add(TrackAddDto objDto)
        {
            Track? obj = await service.SetNextAvailable(service.Validate(objDto.Map()));
            if (obj is null) { return BadRequest(new Track()); }
            else { await service.Add(obj); TrackUniqPropsDto0 objDto0 = new(); objDto0.ReMap(obj); return Ok(await service.GetByUniqProps(objDto0)); }
        }

        [HttpPut("Update")] public async Task<ActionResult<Track>> Update(TrackUpdateDto objDto)
        {
            Track? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(new Track()); }
            else
            {
                obj = await service.SetNextAvailable(service.Validate(objDto.Map(obj)));
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }
    }
}
