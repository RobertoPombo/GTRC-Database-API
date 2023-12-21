using Microsoft.AspNetCore.Mvc;

using GTRC_Database_API.Models;
using GTRC_Database_API.Services;

namespace GTRC_Database_API.Controllers
{
    [ApiController][Route(nameof(Track))]
    public class TrackController(TrackService service, BaseService<Track> baseService) : BaseController<Track>(baseService)
    {
        [HttpGet("{id}")] public ActionResult<Track> GetById(int id)
        {
            Track? obj = service.GetById(id);
            if (obj is null) { return NotFound(service.GetNextAvailable()); }
            else { return Ok(obj); }
        }
    }
}
