﻿using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(User))]
    public class UserController(UserService service, BaseService<User> baseService, FullService<User> fullService) : BaseController<User>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<User?>> GetByUniqProps(UserUniqPropsDto0 objDto)
        {
            UniqPropsDto<User> _objDto = new() { Index = 0, Dto = objDto };
            User? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByUniqProps/1")] public async Task<ActionResult<User?>> GetByUniqProps(UserUniqPropsDto1 objDto)
        {
            UniqPropsDto<User> _objDto = new() { Index = 1, Dto = objDto };
            User? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<User>>> GetByProps(UserAddDto objDto)
        {
            AddDto<User> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<User>>> GetByFilter(UserFilterDtos objDto)
        {
            FilterDtos<User> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<User?>> GetTemp()
        {
            User? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<User?>> Add(UserAddDto objDto)
        {
            User? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<User> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<User?>> Update(UserUpdateDto objDto)
        {
            User? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(User), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpGet("Get/Name3DigitsOptions/{id}")] public async Task<ActionResult<List<string>>> GetName3DigitsOptions(int id)
        {
            User? obj = await baseService.GetById(id);
            if (obj is null) { return NotFound(new List<string>()); }
            else { return Ok(service.GetName3DigitsOptions(obj)); }
        }

        [HttpGet("Get/ByEntry/{entryId}")] public async Task<ActionResult<List<User>>> GetByEntry(int entryId)
        {
            Entry? entry = await fullService.Services[typeof(Entry)].GetById(entryId);
            if (entry is null) { return NotFound(new List<User>()); }
            else { return Ok(await service.GetByEntry(entry)); }
        }

        [HttpGet("Get/ByEntryEvent/{entryId}/{eventId}")] public async Task<ActionResult<List<User>>> GetByEntryEvent(int entryId, int eventId)
        {
            Entry? entry = await fullService.Services[typeof(Entry)].GetById(entryId);
            if (entry is null) { return NotFound(new List<User>()); }
            Event? _event = await fullService.Services[typeof(Event)].GetById(eventId);
            if (_event is null) { return NotFound(new List<User>()); }
            else { return Ok(await service.GetByEntryEvent(entry.Id, _event.Id)); }
        }

        [HttpGet("Get/Violations/DiscordId/{seasonId}")] public async Task<ActionResult<List<User>>> GetViolationsDiscordId(int seasonId)
        {
            Season? season = await fullService.Services[typeof(Season)].GetById(seasonId);
            if (season is null) { return NotFound(new List<User>()); }
            return Ok(await service.GetViolationsDiscordId(season));
        }

        [HttpGet("Get/Violations/AllowEntriesShareDriverSameEvent/{seasonId}")] public async Task<ActionResult<List<User>>> GetViolationsAllowEntriesShareDriverSameEvent(int seasonId)
        {
            Season? season = await fullService.Services[typeof(Season)].GetById(seasonId);
            if (season is null) { return NotFound(new List<User>()); }
            return Ok(await service.GetViolationsAllowEntriesShareDriver(season, true));
        }

        [HttpGet("Get/Violations/AllowEntriesShareDriver/{seasonId}")] public async Task<ActionResult<List<User>>> GetViolationsAllowEntriesShareDriver(int seasonId)
        {
            Season? season = await fullService.Services[typeof(Season)].GetById(seasonId);
            if (season is null) { return NotFound(new List<User>()); }
            return Ok(await service.GetViolationsAllowEntriesShareDriver(season));
        }

        [HttpGet("Get/Violations/ForceDriverFromOrganization/{seasonId}")] public async Task<ActionResult<List<User>>> GetViolationsForceDriverFromOrganization(int seasonId)
        {
            Season? season = await fullService.Services[typeof(Season)].GetById(seasonId);
            if (season is null) { return NotFound(new List<User>()); }
            return Ok(await service.GetViolationsForceDriverFromOrganization(season));
        }
    }
}
