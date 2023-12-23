using System.ComponentModel.DataAnnotations;

using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class TrackUniqPropsDto0 : Mapper<Track>
    {
        [Required] public string AccTrackId { get; set; } = "";

        public override Track Map() { Track obj = new() { AccTrackId = AccTrackId }; return obj; }

        public override void Map(Track obj) { AccTrackId = obj.AccTrackId; }
    }
}
