using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class CarUniqPropsDto0 : Mapper<Car>
    {
        [Required] public uint AccCarId { get; set; } = uint.MinValue;
    }


    public class CarAddDto : Mapper<Car>
    {
        public int? AccCarId { get; set; }
        public string? Name { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public CarClass? Class { get; set; }
        public ushort? Year { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public ushort? WidthMm { get; set; }
        public ushort? LengthMm { get; set; }
        public string? NameGtrc { get; set; }
    }


    public class CarUpdateDto : CarAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;
    }


    public class CarFilterDto : CarAddDto
    {
        public int? Id { get; set; }
    }


    public class CarFilterDtos
    {
        public CarFilterDto Filter { get; set; } = new();
        public CarFilterDto FilterMin { get; set; } = new();
        public CarFilterDto FilterMax { get; set; } = new();
    }
}
