using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(Car))]
    public class CarController(CarService service, BaseService<Car> baseService, FullService<Car> fullService) : BaseController<Car>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<Car?>> GetByUniqProps(CarUniqPropsDto0 objDto)
        {
            UniqPropsDto<Car> _objDto = new() { Index = 0, Dto = objDto };
            Car? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByUniqProps/1")] public async Task<ActionResult<Car?>> GetByUniqProps(CarUniqPropsDto1 objDto)
        {
            UniqPropsDto<Car> _objDto = new() { Index = 1, Dto = objDto };
            Car? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByUniqProps/2")] public async Task<ActionResult<Car?>> GetByUniqProps(CarUniqPropsDto2 objDto)
        {
            UniqPropsDto<Car> _objDto = new() { Index = 2, Dto = objDto };
            Car? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByUniqProps/3")] public async Task<ActionResult<Car?>> GetByUniqProps(CarUniqPropsDto3 objDto)
        {
            UniqPropsDto<Car> _objDto = new() { Index = 3, Dto = objDto };
            Car? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<Car>>> GetByProps(CarAddDto objDto)
        {
            AddDto<Car> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<Car>>> GetByFilter(CarFilterDtos objDto)
        {
            FilterDtos<Car> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<Car?>> GetTemp()
        {
            Car? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<Car?>> Add(CarAddDto objDto)
        {
            Car? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<Car> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<Car?>> Update(CarUpdateDto objDto)
        {
            Car? obj = await service.GetById(objDto.Id);
            if (obj is null) { return NotFound(obj); }
            else
            {
                obj = objDto.Dto2Model(obj);
                bool isValid = service.Validate(obj);
                bool isValidUniqProps = await service.ValidateUniqProps(obj);
                if (obj is null) { return BadRequest(await service.GetById(objDto.Id)); }
                else if (!isValidUniqProps) { return StatusCode(208, obj); }
                else if (!isValid) { return StatusCode(406, obj); }
                else
                {
                    await service.Update(obj);
                    await fullService.UpdateChildObjects(typeof(Car), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpGet("Get/IsLatestModel/{carId}")] public async Task<ActionResult<bool>> GetIsLatestModel(int carId)
        {
            Car? obj = await service.GetById(carId);
            if (obj is null) { return NotFound(false); }
            else { return Ok(await service.GetIsLatestModel(obj)); }
        }
    }
}
