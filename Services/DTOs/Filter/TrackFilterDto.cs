using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class TrackFilterDto : TrackAddDto
    {
        public int? Id { get; set; }

        public override Track Map(Track obj) { obj.Id = Id ?? obj.Id; return base.Map(obj); }

        public override void ReMap(Track obj) { Id = obj.Id; }
    }
}
