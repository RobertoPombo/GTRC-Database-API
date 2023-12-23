using System.ComponentModel.DataAnnotations;

using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class TrackUniqPropsDto0 : Mapper<Track>
    {
        [Required] public string AccTrackId { get; set; } = "";

        public override Track Map(Track obj) { obj.AccTrackId = AccTrackId; return obj; }

        public override void ReMap(Track obj) { AccTrackId = obj.AccTrackId; }
    }
}
