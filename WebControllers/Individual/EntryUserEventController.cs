﻿using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(EntryUserEvent))]
    public class EntryUserEventController(EntryUserEventService service, BaseService<EntryUserEvent> baseService, FullService<EntryUserEvent> fullService) : BaseController<EntryUserEvent>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<EntryUserEvent?>> GetByUniqProps(EntryUserEventUniqPropsDto0 objDto)
        {
            UniqPropsDto<EntryUserEvent> _objDto = new() { Index = 0, Dto = objDto };
            EntryUserEvent? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<EntryUserEvent>>> GetByProps(EntryUserEventAddDto objDto)
        {
            AddDto<EntryUserEvent> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<EntryUserEvent>>> GetByFilter(EntryUserEventFilterDtos objDto)
        {
            FilterDtos<EntryUserEvent> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<EntryUserEvent?>> GetTemp()
        {
            EntryUserEvent? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<EntryUserEvent?>> Add(EntryUserEventAddDto objDto)
        {
            EntryUserEvent? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<EntryUserEvent> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<EntryUserEvent?>> Update(EntryUserEventUpdateDto objDto)
        {
            EntryUserEvent? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(EntryUserEvent), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpGet("Update/Names3Digits/{eventId}")] public async Task<ActionResult<List<EntryUserEvent>>> UpdateNames3Digits(int eventId)
        {
            Event? _event = await fullService.Services[typeof(Event)].GetById(eventId);
            if (_event is null) { return NotFound(new List<EntryUserEvent>()); }
            List<EntryUserEvent> euesToBeUpdated = await service.GetNames3Digits(_event.Id);
            bool isBadRequest = false;
            bool is208 = false;
            bool is406 = false;
            foreach (EntryUserEvent obj in euesToBeUpdated)
            {
                int id = obj.Id;
                bool isValid = service.Validate(obj);
                bool isValidUniqProps = await service.ValidateUniqProps(obj);
                if (obj is null) { isBadRequest = true; }
                else if (!isValidUniqProps) { is208 = true; }
                else if (!isValid) { is406 = true; }
                else { await service.Update(obj); await fullService.UpdateChildObjects(typeof(EntryUserEvent), obj); }
            }
            if (isBadRequest) { return BadRequest(euesToBeUpdated); }
            else if (is208) { return StatusCode(208, euesToBeUpdated); }
            else if (is406) { return StatusCode(406, euesToBeUpdated); }
            else { return Ok(euesToBeUpdated); }
        }
    }
}
