using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

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
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
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
                bool isValidUniqProps = await service.ValidateUniqProps(obj);
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!isValidUniqProps) { return StatusCode(208, obj); }
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

        [HttpGet("Get/ByNr/{seasonId}/{nr}")] public async Task<ActionResult<Event?>> GetByNr(int seasonId, int nr)
        {
            Event? obj = await service.GetByNr(seasonId, nr);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/Next/{seasonId}")] public async Task<ActionResult<Event?>> GetNext(int seasonId, DateTime date)
        {
            Event? obj = await service.GetNext(seasonId, date);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/First/{seasonId}")] public async Task<ActionResult<Event?>> GetFirst(int seasonId)
        {
            Event? _event = await service.GetFirst(seasonId, false);
            if (_event is null) { return NotFound(null); }
            else { return Ok(_event); }
        }

        [HttpGet("Get/Final/{seasonId}")] public async Task<ActionResult<Event?>> GetFinal(int seasonId)
        {
            Event? _event = await service.GetFirst(seasonId, true);
            if (_event is null) { return NotFound(null); }
            else { return Ok(_event); }
        }

        [HttpGet("Get/IsOver/{id}")] public async Task<ActionResult<bool>> GetIsOver(int id)
        {
            Event? _event = await service.GetById(id);
            if (_event is null) { return NotFound(false); }
            else { return Ok(await service.GetIsOver(_event)); }
        }
    }
}
