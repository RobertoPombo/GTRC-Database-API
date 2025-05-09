﻿using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Team))]
    public class TeamController(TeamService service, BaseService<Team> baseService, FullService<Team> fullService) : BaseController<Team>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Team?>> GetByUniqProps(TeamUniqPropsDto0 objDto)
        {
            UniqPropsDto<Team> _objDto = new() { Index = 0, Dto = objDto };
            Team? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Team>>> GetByProps(TeamAddDto objDto)
        {
            AddDto<Team> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Team>>> GetByFilter(TeamFilterDtos objDto)
        {
            FilterDtos<Team> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Team?>> GetTemp()
        {
            Team? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Team?>> Add(TeamAddDto objDto)
        {
            Team? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Team> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Team?>> Update(TeamUpdateDto objDto)
        {
            Team? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(Team), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpGet("Get/BySeason/{seasonId}")] public async Task<ActionResult<List<Team>>> GetBySeason(int seasonId)
        {
            Season? season = await fullService.Services[typeof(Season)].GetById(seasonId);
            if (season is null) { return NotFound(new List<Team>()); }
            return Ok(await service.GetBySeason(season));
        }

        [HttpGet("Get/Violations/MinEntriesPerTeam/{seasonId}")] public async Task<ActionResult<List<Team>>> GetViolationsMinEntriesPerTeam(int seasonId)
        {
            Season? season = await fullService.Services[typeof(Season)].GetById(seasonId);
            if (season is null) { return NotFound(new List<Team>()); }
            return Ok(await service.GetViolationsMinEntriesPerTeam(season));
        }

        [HttpGet("Get/Violations/MaxEntriesPerTeam/{seasonId}")] public async Task<ActionResult<List<Team>>> GetViolationsMaxEntriesPerTeam(int seasonId)
        {
            Season? season = await fullService.Services[typeof(Season)].GetById(seasonId);
            if (season is null) { return NotFound(new List<Team>()); }
            return Ok(await service.GetViolationsMinEntriesPerTeam(season, true));
        }
    }
}
