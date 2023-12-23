using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class TrackUniqPropsDto0 : Mapper<Track>
    {
        [Required] public string AccTrackId { get; set; } = "";

        public override Track Map(Track obj) { obj.AccTrackId = AccTrackId; return obj; }

        public override void ReMap(Track obj) { AccTrackId = obj.AccTrackId; }
    }


    public class TrackAddDto : Mapper<Track>
    {
        public string? AccTrackId { get; set; }
        public string? Name { get; set; }
        public int? PitBoxesCount { get; set; }
        public int? ServerSlotsCount { get; set; }
        public int? AccTimePenDT { get; set; }
        public string? NameGtrc { get; set; }

        public override Track Map(Track obj)
        {
            obj.AccTrackId = AccTrackId ?? obj.AccTrackId;
            obj.Name = Name ?? obj.Name;
            obj.PitBoxesCount = PitBoxesCount ?? obj.PitBoxesCount;
            obj.ServerSlotsCount = ServerSlotsCount ?? obj.ServerSlotsCount;
            obj.AccTimePenDT = AccTimePenDT ?? obj.AccTimePenDT;
            obj.NameGtrc = NameGtrc ?? obj.NameGtrc;
            return obj;
        }

        public override void ReMap(Track obj)
        {
            AccTrackId = obj.AccTrackId;
            Name = obj.Name;
            PitBoxesCount = obj.PitBoxesCount;
            ServerSlotsCount = obj.ServerSlotsCount;
            AccTimePenDT = obj.AccTimePenDT;
            NameGtrc = obj.NameGtrc;
        }
    }


    public class TrackUpdateDto : TrackAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;

        public override Track Map(Track obj) { obj.Id = Id; return base.Map(obj); }

        public override void ReMap(Track obj) { base.ReMap(obj); obj.Id = Id; }
    }


    public class TrackFilterDto : TrackAddDto
    {
        public int? Id { get; set; }

        public override Track Map(Track obj) { obj.Id = Id ?? obj.Id; return base.Map(obj); }

        public override void ReMap(Track obj) { Id = obj.Id; }
    }


    public class TrackFilterDtos
    {
        public TrackFilterDto Filter { get; set; } = new();
        public TrackFilterDto FilterMin { get; set; } = new();
        public TrackFilterDto FilterMax { get; set; } = new();
    }
}
