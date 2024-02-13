using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;
using GTRC_Basics;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(BopTrackCar))]
    public class BopTrackCarController(BopTrackCarService service, BaseService<BopTrackCar> baseService, FullService<BopTrackCar> fullService) : BaseController<BopTrackCar>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<BopTrackCar?>> GetByUniqProps(BopTrackCarUniqPropsDto0 objDto)
        {
            UniqPropsDto<BopTrackCar> _objDto = new() { Index = 0, Dto = objDto };
            BopTrackCar? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<BopTrackCar>>> GetByProps(BopTrackCarAddDto objDto)
        {
            AddDto<BopTrackCar> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<BopTrackCar>>> GetByFilter(BopTrackCarFilterDtos objDto)
        {
            FilterDtos<BopTrackCar> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<BopTrackCar?>> GetTemp()
        {
            BopTrackCar? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<BopTrackCar?>> Add(BopTrackCarAddDto objDto)
        {
            BopTrackCar? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isAvailable = await service.SetNextAvailable(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isAvailable) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<BopTrackCar> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<BopTrackCar?>> Update(BopTrackCarUpdateDto objDto)
        {
            BopTrackCar? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = objDto.Dto2Model(obj);
                bool isValid = service.Validate(obj);
                bool isAvailable = await service.SetNextAvailable(obj);
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!isAvailable) { return StatusCode(208, await service.SetNextAvailable(obj)); }
                else if (!isValid) { return StatusCode(406, obj); }
                else
                {
                    await service.Update(obj);
                    await fullService.UpdateChildObjects(typeof(BopTrackCar), obj);
                    return Ok(obj);
                }
            }
        }
    }
}
