using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class CarUniqPropsDto0 : Mapper<Car>
    {
        [Required] public int AccCarId { get; set; } = GlobalValues.NoID;

        public override Car Map(Car obj) { obj.AccCarId = AccCarId; return obj; }

        public override void ReMap(Car obj) { AccCarId = obj.AccCarId; }
    }


    public class CarAddDto : Mapper<Car>
    {
        public int? AccCarId { get; set; }
        public string? Name { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public CarClass? Class { get; set; }
        public int? Year { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public int? WidthMm { get; set; }
        public int? LengthMm { get; set; }
        public string? NameGtrc { get; set; }

        public override Car Map(Car obj)
        {
            obj.AccCarId = AccCarId ?? obj.AccCarId;
            obj.Name = Name ?? obj.Name;
            obj.Manufacturer = Manufacturer ?? obj.Manufacturer;
            obj.Model = Model ?? obj.Model;
            obj.Class = Class ?? obj.Class;
            obj.Year = Year ?? obj.Year;
            obj.ReleaseDate = ReleaseDate ?? obj.ReleaseDate;
            obj.WidthMm = WidthMm ?? obj.WidthMm;
            obj.LengthMm = LengthMm ?? obj.LengthMm;
            obj.NameGtrc = NameGtrc ?? obj.NameGtrc;
            return obj;
        }

        public override void ReMap(Car obj)
        {
            AccCarId = obj.AccCarId;
            Name = obj.Name;
            Manufacturer = obj.Manufacturer;
            Model = obj.Model;
            Class = obj.Class;
            Year = obj.Year;
            ReleaseDate = obj.ReleaseDate;
            WidthMm = obj.WidthMm;
            LengthMm = obj.LengthMm;
            NameGtrc = obj.NameGtrc;
        }
    }


    public class CarUpdateDto : CarAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;

        public override Car Map(Car obj) { obj.Id = Id; return base.Map(obj); }

        public override void ReMap(Car obj) { base.ReMap(obj); obj.Id = Id; }
    }


    public class CarFilterDto : CarAddDto
    {
        public int? Id { get; set; }

        public override Car Map(Car obj) { obj.Id = Id ?? obj.Id; return base.Map(obj); }

        public override void ReMap(Car obj) { Id = obj.Id; }
    }


    public class CarFilterDtos
    {
        public CarFilterDto Filter { get; set; } = new();
        public CarFilterDto FilterMin { get; set; } = new();
        public CarFilterDto FilterMax { get; set; } = new();
    }
}
