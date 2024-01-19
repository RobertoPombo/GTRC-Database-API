using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Event))]
    public class EventController(EventService service, BaseService<Event> baseService) : BaseController<Event>(baseService)
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
            Event? objValidated = service.Validate(objDto.Dto2Model());
            Event? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null || objValidated is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
            else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
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
                Event? objValidated = service.Validate(objDto.Dto2Model(obj));
                obj = await service.SetNextAvailable(objDto.Dto2Model(obj));
                if (obj is null || objValidated is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
                else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }

        [HttpDelete("Delete/{id}/{force}")] public async Task<ActionResult> Delete(int id, bool force = false)
        {
            Event? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await service.HasChildObjects(obj.Id)) { return StatusCode(405); }
            else { await service.Delete(obj); return Ok(); }
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
