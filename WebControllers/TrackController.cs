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
            else { await service.Add(obj); TrackUniqPropsDto0 objDto0 = new(); objDto0.Map(obj); return Ok(await service.GetByUniqProps(objDto0)); }
        }
    }
}
