using Microsoft.AspNetCore.Mvc;

using GTRC_Database_API.Models;
using GTRC_Database_API.Services;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController(CarService service) : ControllerBase
    {

        [HttpGet()] public ActionResult<List<Car>> GetAll() { return Ok(service.GetAll()); }

        [HttpGet("{id}")] public ActionResult<Car> GetById(int id)
        {
            Car? car = service.GetById(id);
            if (car is null) { return NotFound(service.GetNextAvailable()); }
            else { return Ok(car); }
        }
    }
}
