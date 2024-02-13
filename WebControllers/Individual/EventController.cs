using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;
using GTRC_Basics;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Event))]
    public class EventController(EventService service, BaseService<Event> baseService, FullService<Event> fullService) : BaseController<Event>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Event?>> GetByUniqProps(EventUniqPropsDto0 objDto)
        {
            UniqPropsDto<Event> _objDto = new() { Index = 0, Dto = objDto };
            Event? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByUniqProps/1")] public async Task<ActionResult<Event?>> GetByUniqProps(EventUniqPropsDto1 objDto)
        {
            UniqPropsDto<Event> _objDto = new() { Index = 1, Dto = objDto };
            Event? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Event>>> GetByProps(EventAddDto objDto)
        {
            AddDto<Event> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Event>>> GetByFilter(EventFilterDtos objDto)
        {
            FilterDtos<Event> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Event?>> GetTemp()
        {
            Event? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Event?>> Add(EventAddDto objDto)
        {
            Event? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isAvailable = await service.SetNextAvailable(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isAvailable) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Event> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Event?>> Update(EventUpdateDto objDto)
        {
            Event? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = objDto.Dto2Model(obj);
                bool isValid = service.Validate(obj);
                bool isAvailable = await service.SetNextAvailable(obj);
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!isAvailable) { return StatusCode(208, obj); }
                else if (!isValid) { return StatusCode(406, obj); }
                else
                {
                    await service.Update(obj);
                    await fullService.UpdateChildObjects(typeof(Event), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpGet("Get/Nr/{id}")] public async Task<ActionResult<int?>> GetNr(int id)
        {
            int? nr = await service.GetNr(id);
            if (nr is null) { return NotFound(nr); }
            else { return Ok(nr); }
        }

        [HttpGet("Get/Next/{seasonId}/{date}")] public async Task<ActionResult<Event?>> GetNext(int seasonId, DateTime? date = null)
        {
            Event? obj = await service.GetNext(seasonId, date);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }
    }
}
