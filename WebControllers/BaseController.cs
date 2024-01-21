using Microsoft.AspNetCore.Mvc;

using GTRC_Database_API.Services;
using GTRC_Basics.Models.Common;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(ModelType))]
    public class BaseController<ModelType>(BaseService<ModelType> service) : ControllerBase where ModelType : class, IBaseModel, new()
    {
        [HttpGet("Get")] public async Task<ActionResult<List<ModelType>>> GetAll() { return Ok(await service.GetAll()); }

        [HttpGet("Get/{id}")] public async Task<ActionResult<ModelType?>> GetById(int id)
        {
            ModelType? obj = await service.GetById(id);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }
    }
}
