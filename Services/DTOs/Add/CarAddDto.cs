namespace GTRC_Database_API.Services.DTOs
{
    public class CarAddDto
    {
        public int? AccCarId { get; set; }
        public string? Name { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? Category { get; set; }
        public int? Year { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? NameGtrc { get; set; }
    }
}
