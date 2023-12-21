using Microsoft.AspNetCore.Mvc;

using GTRC_Database_API.Models;
using GTRC_Database_API.Services;

namespace GTRC_Database_API.Controllers
{
    [ApiController][Route(nameof(ModelType))]
    public class BaseController<ModelType>(BaseService<ModelType> service) : ControllerBase where ModelType : class, IBaseModel
    {
        [HttpGet()] public ActionResult<List<ModelType>> GetAll() { return Ok(service.GetAll()); }
    }
}
