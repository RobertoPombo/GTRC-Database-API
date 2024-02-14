using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;
using GTRC_Basics;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(SeasonCarclass))]
    public class SeasonCarclassController(SeasonCarclassService service, BaseService<SeasonCarclass> baseService, FullService<SeasonCarclass> fullService) : BaseController<SeasonCarclass>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<SeasonCarclass?>> GetByUniqProps(SeasonCarclassUniqPropsDto0 objDto)
        {
            UniqPropsDto<SeasonCarclass> _objDto = new() { Index = 0, Dto = objDto };
            SeasonCarclass? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<SeasonCarclass>>> GetByProps(SeasonCarclassAddDto objDto)
        {
            AddDto<SeasonCarclass> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<SeasonCarclass>>> GetByFilter(SeasonCarclassFilterDtos objDto)
        {
            FilterDtos<SeasonCarclass> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<SeasonCarclass?>> GetTemp()
        {
            SeasonCarclass? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<SeasonCarclass?>> Add(SeasonCarclassAddDto objDto)
        {
            SeasonCarclass? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<SeasonCarclass> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<SeasonCarclass?>> Update(SeasonCarclassUpdateDto objDto)
        {
            SeasonCarclass? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(SeasonCarclass), obj);
                    return Ok(obj);
                }
            }
        }
    }
}
