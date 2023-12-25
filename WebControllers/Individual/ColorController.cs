using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Color))]
    public class ColorController(ColorService service, BaseService<Color> baseService) : BaseController<Color>(baseService)
    {
        [HttpGet("Get/ByUniqProps")]
        public async Task<ActionResult<Color?>> GetByUniqProps([FromQuery] ColorUniqPropsDto0 objDto)
        {
            Color? obj = await service.GetByUniqProps(objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/ByProps")]
        public async Task<ActionResult<List<Color>>> GetByProps([FromQuery] ColorAddDto objDto)
        {
            return Ok(await service.GetByProps(objDto));
        }

        [HttpGet("Get/ByFilter")]
        public async Task<ActionResult<List<Color>>> GetByFilter([FromQuery] ColorFilterDtos objDto)
        {
            return Ok(await service.GetByFilter(objDto.Filter, objDto.FilterMin, objDto.FilterMax));
        }

        [HttpGet("Get/Temp")]
        public async Task<ActionResult<Color?>> GetTemp()
        {
            Color? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Color?>> Add(ColorAddDto objDto)
        {
            Color? obj = await service.SetNextAvailable(ColorService.Validate(objDto.Map()));
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
            else { await service.Add(obj); ColorUniqPropsDto0 objDto0 = new(); objDto0.ReMap(obj); return Ok(await service.GetByUniqProps(objDto0)); }
        }

        [HttpPut("Update")]
        public async Task<ActionResult<Color?>> Update(ColorUpdateDto objDto)
        {
            Color? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = await service.SetNextAvailable(ColorService.Validate(objDto.Map(obj)));
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }
    }
}
