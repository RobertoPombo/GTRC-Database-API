using GTRC_Database_API.Models.Common;

namespace GTRC_Database_API.Models
{
    public class Car : BaseModel
    {
        static Car() { UniqProps = [[typeof(Car).GetProperty(nameof(AccCarID))!]]; }

        public int AccCarID { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";
        public string Category { get; set; } = "";
        public int Year { get; set; } = DateTime.Now.Year;
        public DateOnly ReleaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Name_GTRC { get; set; } = "";
    }
}
