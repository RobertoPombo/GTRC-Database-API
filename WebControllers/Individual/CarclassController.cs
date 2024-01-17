using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Carclass))]
    public class CarclassController(CarclassService service, BaseService<Carclass> baseService) : BaseController<Carclass>(baseService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Carclass?>> GetByUniqProps(CarclassUniqPropsDto0 objDto)
        {
            UniqPropsDto<Carclass> _objDto = new() { Index = 0, Dto = objDto };
            Carclass? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Carclass>>> GetByProps(CarclassAddDto objDto)
        {
            AddDto<Carclass> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Carclass>>> GetByFilter(CarclassFilterDtos objDto)
        {
            FilterDtos<Carclass> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Carclass?>> GetTemp()
        {
            Carclass? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Carclass?>> Add(CarclassAddDto objDto)
        {
            Carclass? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Carclass> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Carclass?>> Update(CarclassUpdateDto objDto)
        {
            Carclass? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = await service.SetNextAvailable(objDto.Dto2Model(obj));
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(obj)) { return Conflict(obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }

        [HttpDelete("Delete/{id}/{force}")] public async Task<ActionResult> Delete(int id, bool force = false)
        {
            Carclass? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await service.HasChildObjects(obj.Id)) { return Unauthorized(); }
            else { await service.Delete(obj); return Ok(); }
        }
    }
}
