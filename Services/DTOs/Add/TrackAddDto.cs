using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class TrackAddDto : Mapper<Track>
    {
        public string? AccTrackId { get; set; }
        public string? Name { get; set; }
        public int? PitBoxesCount { get; set; }
        public int? ServerSlotsCount { get; set; }
        public int? AccTimePenDT { get; set; }
        public string? NameGtrc { get; set; }

        public override Track Map()
        {
            Track obj = new();
            obj.AccTrackId = AccTrackId ?? obj.AccTrackId;
            obj.Name = Name ?? obj.Name;
            obj.PitBoxesCount = PitBoxesCount ?? obj.PitBoxesCount;
            obj.ServerSlotsCount = ServerSlotsCount ?? obj.ServerSlotsCount;
            obj.AccTimePenDT = AccTimePenDT ?? obj.AccTimePenDT;
            obj.NameGtrc = NameGtrc ?? obj.NameGtrc;
            return obj;
        }

        public override void Map(Track obj)
        {
            AccTrackId = obj.AccTrackId;
            Name = obj.Name;
            PitBoxesCount = obj.PitBoxesCount;
            ServerSlotsCount = obj.ServerSlotsCount;
            AccTimePenDT = obj.AccTimePenDT;
            NameGtrc = obj.NameGtrc;
        }
    }
}
