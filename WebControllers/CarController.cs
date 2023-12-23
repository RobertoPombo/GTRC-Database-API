using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Database_API.Services.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController][Route(nameof(Car))]
    public class CarController(CarService service, BaseService<Car> baseService) : BaseController<Car>(baseService)
    {
        [HttpGet("Get/ByUniqProps")] public async Task<ActionResult<Car>> GetByUniqProps([FromQuery] CarUniqPropsDto0 objDto)
        {
            Car? obj = await service.GetByUniqProps(objDto);
            if (obj is null) { return NotFound(new Car()); }
            else { return Ok(obj); }
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Car>> GetTemp()
        {
            Car? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(new Car()); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Car>> Add(CarAddDto objDto)
        {
            Car? obj = await service.SetNextAvailable(service.Validate(objDto.Map()));
            if (obj is null) { return BadRequest(new Car()); }
            else { await service.Add(obj); CarUniqPropsDto0 objDto0 = new(); objDto0.ReMap(obj); return Ok(await service.GetByUniqProps(objDto0)); }
        }

        [HttpPut("Update")] public async Task<ActionResult<Car>> Update(CarUpdateDto objDto)
        {
            Car? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(new Car()); }
            else
            {
                obj = await service.SetNextAvailable(service.Validate(objDto.Map(obj)));
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else { await service.Update(obj); return Ok(obj); }
            }
        }
    }
}
