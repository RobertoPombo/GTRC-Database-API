using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Track))]
    public class TrackController(TrackService service, BaseService<Track> baseService) : BaseController<Track>(baseService)
    {
        [HttpGet("Get/Temp")] public async Task<ActionResult<Track?>> GetTemp()
        {
            Track? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Track?>> Add(AddDto<Track> objDto)
        {
            Track? obj = await service.SetNextAvailable(TrackService.Validate(objDto.Dto.Map()));
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.Dto.IsSimilar(obj)) { return Conflict(obj); }
            else { await service.Add(obj); UniqPropsDto<Track> uniqPropsDto = new(); uniqPropsDto.Dto.ReMap(obj); return Ok(await service.GetByUniqProps(uniqPropsDto)); }
        }

        [HttpPut("Update")] public async Task<ActionResult<Track?>> Update(UpdateDto<Track> objDto)
        {
            Track? obj = await service.GetById(objDto.Dto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = await service.SetNextAvailable(TrackService.Validate(objDto.Dto.Map(obj)));
                if (obj is null) { return BadRequest(await service.GetById(objDto.Dto.Id)); }
                else if (!objDto.Dto.IsSimilar(obj)) { return Conflict(obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }
    }
}
