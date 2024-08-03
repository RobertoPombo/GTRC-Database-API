using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Resultsfile))]
    public class ResultsfileController(ResultsfileService service, BaseService<Resultsfile> baseService, FullService<Resultsfile> fullService) : BaseController<Resultsfile>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Resultsfile?>> GetByUniqProps(ResultsfileUniqPropsDto0 objDto)
        {
            UniqPropsDto<Resultsfile> _objDto = new() { Index = 0, Dto = objDto };
            Resultsfile? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByUniqProps/1")] public async Task<ActionResult<Resultsfile?>> GetByUniqProps(ResultsfileUniqPropsDto1 objDto)
        {
            UniqPropsDto<Resultsfile> _objDto = new() { Index = 1, Dto = objDto };
            Resultsfile? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Resultsfile>>> GetByProps(ResultsfileAddDto objDto)
        {
            AddDto<Resultsfile> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Resultsfile>>> GetByFilter(ResultsfileFilterDtos objDto)
        {
            FilterDtos<Resultsfile> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Resultsfile?>> GetTemp()
        {
            Resultsfile? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Resultsfile?>> Add(ResultsfileAddDto objDto)
        {
            Resultsfile? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Resultsfile> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Resultsfile?>> Update(ResultsfileUpdateDto objDto)
        {
            Resultsfile? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(Resultsfile), obj);
                    return Ok(obj);
                }
            }
        }
    }
}
