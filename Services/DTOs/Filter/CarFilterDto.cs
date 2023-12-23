using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class CarFilterDto : CarAddDto
    {
        public int? Id { get; set; }

        public override Car Map(Car obj) { obj.Id = Id ?? obj.Id; return base.Map(obj); }

        public override void ReMap(Car obj) { Id = obj.Id; }
    }
}
