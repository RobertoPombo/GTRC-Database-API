using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;
using GTRC_Basics;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(EntryEvent))]
    public class EntryEventController(EntryEventService service, BaseService<EntryEvent> baseService, FullService<EntryEvent> fullService) : BaseController<EntryEvent>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<EntryEvent?>> GetByUniqProps(EntryEventUniqPropsDto0 objDto)
        {
            UniqPropsDto<EntryEvent> _objDto = new() { Index = 0, Dto = objDto };
            EntryEvent? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<EntryEvent>>> GetByProps(EntryEventAddDto objDto)
        {
            AddDto<EntryEvent> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<EntryEvent>>> GetByFilter(EntryEventFilterDtos objDto)
        {
            FilterDtos<EntryEvent> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<EntryEvent?>> GetTemp()
        {
            EntryEvent? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<EntryEvent?>> Add(EntryEventAddDto objDto)
        {
            EntryEvent? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<EntryEvent> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<EntryEvent?>> Update(EntryEventUpdateDto objDto)
        {
            EntryEvent? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(EntryEvent), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpPut("Get/ByUniqProps/0/Any")] public async Task<ActionResult<EntryEvent?>> GetAnyByUniqProps(EntryEventUniqPropsDto0 objDto)
        {
            UniqPropsDto<EntryEvent> uniqDto = new() { Dto = objDto };
            EntryEvent? obj = await service.GetByUniqProps(uniqDto);
            if (obj is not null) { return Ok(obj); }
            else 
            {
                Entry entry = await fullService.Services[typeof(Entry)].GetById(objDto.EntryId);
                Event _event = await fullService.Services[typeof(Event)].GetById(objDto.EventId);
                if (entry is null || _event is null) { return NotFound(null); }
                else
                {
                    DateTime signInDate = GlobalValues.DateTimeMaxValue;
                    if (EntryFullDto.GetRegisterState(entry) && entry.IsPermanent) { signInDate = GlobalValues.DateTimeMinValue; }
                    EntryEvent newObj = new()
                    {
                        EntryId = entry.Id,
                        EventId = _event.Id,
                        SignInDate = signInDate,
                        IsPointScorer = entry.IsPointScorer
                    };
                    await service.ValidateUniqProps(newObj);
                    if (newObj is not null) { return Ok(newObj); }
                    else { return StatusCode(406, newObj); }
                }
            }
        }
    }
}
