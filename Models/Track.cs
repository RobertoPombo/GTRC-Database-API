using GTRC_Database_API.Models.Common;

namespace GTRC_Database_API.Models
{
    public class Track : BaseModel
    {
        public static readonly string DefaultAccTrackID = "TrackID";
        static Track() { UniqProps = [[typeof(Track).GetProperty(nameof(AccTrackID))!]]; }

        public string AccTrackID { get; set; } = DefaultAccTrackID;
        public string Name { get; set; } = "";
        public int PitBoxesCount { get; set; } = 0;
        public int ServerSlotsCount { get; set; } = 0;
        public int AccTimePenDT { get; set; } = 30;
        public string Name_GTRC { get; set; } = "";
    }
}
