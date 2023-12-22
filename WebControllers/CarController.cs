using Microsoft.AspNetCore.Mvc;

using GTRC_Database_API.Models;
using GTRC_Database_API.Services;

namespace GTRC_Database_API.Controllers
{
    [ApiController][Route(nameof(Car))]
    public class CarController(CarService service, BaseService<Car> baseService) : BaseController<Car>(baseService)
    {

    }
}
