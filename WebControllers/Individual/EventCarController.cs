using Microsoft.AspNetCore.Mvc;

using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Controllers
{
    [ApiController]
    [Route(nameof(EventCar))]
    public class EventCarController(EventCarService service, BaseService<EventCar> baseService, FullService<EventCar> fullService) : BaseController<EventCar>(baseService, fullService)
    {
        [HttpPut("Get/ByUniqProps/0")] public async Task<ActionResult<EventCar?>> GetByUniqProps(EventCarUniqPropsDto0 objDto)
        {
            UniqPropsDto<EventCar> _objDto = new() { Index = 0, Dto = objDto };
            EventCar? obj = await service.GetByUniqProps(_objDto);
            if (obj is null) { return NotFound(obj); }
            else { return Ok(obj); }
        }

        [HttpPut("Get/ByProps")] public async Task<ActionResult<List<EventCar>>> GetByProps(EventCarAddDto objDto)
        {
            AddDto<EventCar> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByProps(_objDto));
        }

        [HttpPut("Get/ByFilter")] public async Task<ActionResult<List<EventCar>>> GetByFilter(EventCarFilterDtos objDto)
        {
            FilterDtos<EventCar> _objDto = new() { Dto = objDto };
            return Ok(await service.GetByFilter(_objDto.Filter, _objDto.FilterMin, _objDto.FilterMax));
        }

        [HttpGet("Get/Temp")] public async Task<ActionResult<EventCar?>> GetTemp()
        {
            EventCar? obj = await service.GetTemp();
            if (obj is null) { return BadRequest(obj); }
            else { return Ok(obj); }
        }

        [HttpPost("Add")] public async Task<ActionResult<EventCar?>> Add(EventCarAddDto objDto)
        {
            EventCar? obj = objDto.Dto2Model();
            bool isValid = service.Validate(obj);
            bool isValidUniqProps = await service.ValidateUniqProps(obj);
            if (obj is null) { return BadRequest(obj); }
            else if (!isValidUniqProps) { return StatusCode(208, obj); }
            else if (!isValid) { return StatusCode(406, obj); }
            else
            {
                await service.Add(obj);
                UniqPropsDto<EventCar> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);
                obj = await service.GetByUniqProps(uniqPropsDto);
                if (obj is null) { return NotFound(obj); }
                else { return Ok(obj); }
            }
        }

        [HttpPut("Update")] public async Task<ActionResult<EventCar?>> Update(EventCarUpdateDto objDto)
        {
            EventCar? obj = await service.GetById(objDto.Id);
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
                    await fullService.UpdateChildObjects(typeof(EventCar), obj);
                    return Ok(obj);
                }
            }
        }

        [HttpGet("Update/Count/{eventId}")] public async Task<ActionResult<List<EventCar>>> UpdateCount(int eventId)
        {
            Event? _event = await fullService.Services[typeof(Event)].GetById(eventId);
            if (_event is null) { return NotFound(new List<EventCar>()); }
            List<EventCar> list = [];
            (List<EventCar> eventCarsToBeAdded, List<EventCar> eventCarsToBeUpdated) = await service.CountCars(_event);
            bool isNotFound = false;
            bool isBadRequest = false;
            bool is208 = false;
            bool is406 = false;
            foreach (EventCar obj in eventCarsToBeUpdated) { list.Add(obj); }
            foreach (EventCar obj in eventCarsToBeAdded)
            {
                int id = obj.Id;
                bool isValid = service.Validate(obj);
                bool isValidUniqProps = await service.ValidateUniqProps(obj);
                if (obj is null) { isBadRequest = true; }
                else if (!isValidUniqProps) { is208 = true; }
                else if (!isValid) { is406 = true; }
                else
                {
                    await service.Add(obj);
                    UniqPropsDto<EventCar> uniqPropsDto = new();
                    uniqPropsDto.Dto.Model2Dto(obj);
                    EventCar? _obj = await service.GetByUniqProps(uniqPropsDto);
                    if (_obj is null) { isNotFound = true; }
                    else { list.Add(_obj); }
                }
            }
            if (isNotFound) { return NotFound(list); }
            else if (isBadRequest) { return BadRequest(list); }
            else if (is208) { return StatusCode(208, list); }
            else if (is406) { return StatusCode(406, list); }
            else
            {
                list = await service.CalculateBop(list);
                foreach (EventCar obj in list)
                {
                    int id = obj.Id;
                    bool isValid = service.Validate(obj);
                    bool isValidUniqProps = await service.ValidateUniqProps(obj);
                    if (obj is null) { isBadRequest = true; }
                    else if (!isValidUniqProps) { is208 = true; }
                    else if (!isValid) { is406 = true; }
                    else { await service.Update(obj); await fullService.UpdateChildObjects(typeof(Entry), obj); }
                }
                if (isBadRequest) { return BadRequest(list); }
                else if (is208) { return StatusCode(208, list); }
                else if (is406) { return StatusCode(406, list); }
                else { return Ok(list); }
            }
        }
    }
}
