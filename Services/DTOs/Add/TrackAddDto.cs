namespace GTRC_Database_API.Services.DTOs
{
    public class TrackAddDto
    {
        public string? AccTrackId { get; set; }
        public string? Name { get; set; }
        public int? PitBoxesCount { get; set; }
        public int? ServerSlotsCount { get; set; }
        public int? AccTimePenDT { get; set; }
        public string? NameGtrc { get; set; }
    }
}
