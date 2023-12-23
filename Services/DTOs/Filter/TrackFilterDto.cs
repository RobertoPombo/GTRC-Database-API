using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class TrackFilterDto : TrackAddDto
    {
        public int? Id { get; set; }

        public override Track Map() { Track obj = base.Map(); obj.Id = Id ?? obj.Id; return obj; }

        public override void Map(Track obj) { Id = obj.Id; }
    }
}
