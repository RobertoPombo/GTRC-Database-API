using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class CarFilterDto : CarAddDto
    {
        public int? Id { get; set; }

        public override Car Map() { Car obj = base.Map(); obj.Id = Id ?? obj.Id; return obj; }

        public override void Map(Car obj) { Id = obj.Id; }
    }
}
