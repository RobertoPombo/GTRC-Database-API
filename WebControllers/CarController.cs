using Microsoft.AspNetCore.Mvc;

using GTRC_Database_API.Models;
using GTRC_Database_API.Services;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController(CarService carService) : ControllerBase
    {

        [HttpGet()]
        public ActionResult<List<Car>> GetAll() { return Ok(carService.GetAllAsync()); }
    }
}
