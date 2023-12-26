using Microsoft.AspNetCore.Mvc;

using GTRC_Database_API.Services;
using GTRC_Basics.Models.Common;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController][Route(nameof(ModelType))]
    public class BaseController<ModelType>(BaseService<ModelType> service) : ControllerBase where ModelType : class, IBaseModel, new()
    {
        [HttpGet("Get")] public async Task<ActionResult<List<ModelType>>> GetAll() { return Ok(await service.GetAll()); }

        [HttpGet("Get/{id}")] public async Task<ActionResult<ModelType?>> GetById(int id)
        {
            ModelType? obj = await service.GetById(id);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/ByUniqProps")] public async Task<ActionResult<ModelType?>> GetByUniqProps([FromQuery] UniqPropsDto<ModelType> objDto)
        {
            ModelType? obj = await service.GetByUniqProps(objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/ByProps")] public async Task<ActionResult<List<ModelType>>> GetByProps([FromQuery] AddDto<ModelType> objDto)
        {
            return Ok(await service.GetByProps(objDto));
        }

        [HttpGet("Get/ByFilter")] public async Task<ActionResult<List<ModelType>>> GetByFilter([FromQuery] FilterDtos<ModelType> objDto)
        {
            return Ok(await service.GetByFilter(objDto.Filter, objDto.FilterMin, objDto.FilterMax));
        }

        [HttpDelete("Delete/{id}/{force}")] public async Task<ActionResult> Delete(int id, bool force=false)
        {
            ModelType? obj = await service.GetById(id);
            if (obj is null) { return NotFound(); }
            else if (!force) { return Unauthorized(); }
            else
            {
                await service.Delete(obj);
                return Ok();
            }
        }
    }
}
