using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Entry))]
    public class EntryController(EntryService service, BaseService<Entry> baseService) : BaseController<Entry>(baseService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Entry?>> GetByUniqProps(EntryUniqPropsDto0 objDto)
        {
            UniqPropsDto<Entry> _objDto = new() { Index = 0, Dto = objDto };
            Entry? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Entry>>> GetByProps(EntryAddDto objDto)
        {
            AddDto<Entry> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Entry>>> GetByFilter(EntryFilterDtos objDto)
        {
            FilterDtos<Entry> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Entry?>> GetTemp()
        {
            Entry? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Entry?>> Add(EntryAddDto objDto)
        {
            Entry? objValidated = service.Validate(objDto.Dto2Model());
            Entry? obj = await service.SetNextAvailable(objDto.Dto2Model());
            if (obj is null || objValidated is null) { return BadRequest(obj); }
            else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
            else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Entry> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Entry?>> Update(EntryUpdateDto objDto)
        {
            Entry? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                Entry? objValidated = service.Validate(objDto.Dto2Model(obj));
                obj = await service.SetNextAvailable(objDto.Dto2Model(obj));
                if (obj is null || objValidated is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!objDto.IsSimilar(objValidated)) { return StatusCode(406, obj); }
                else if (!objDto.IsSimilar(obj)) { return StatusCode(208, obj); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }

        [HttpDelete("Delete/{id}/{force}")] public async Task<ActionResult> Delete(int id, bool force = false)
        {
            Entry? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force && await service.HasChildObjects(obj.Id)) { return StatusCode(405); }
            else { await service.Delete(obj); return Ok(); }
        }
    }
}
