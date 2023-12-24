using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class ColorUniqPropsDto0 : Mapper<Color>
    {
        [Required] public byte Alpha { get; set; } = byte.MinValue;
        [Required] public byte Red { get; set; } = byte.MinValue;
        [Required] public byte Green { get; set; } = byte.MinValue;
        [Required] public byte Blue { get; set; } = byte.MinValue;
    }


    public class ColorAddDto : Mapper<Color>
    {
        public byte? Alpha { get; set; }
        public byte? Red { get; set; }
        public byte? Green { get; set; }
        public byte? Blue { get; set; }
    }


    public class ColorUpdateDto : ColorAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;
    }


    public class ColorFilterDto : ColorAddDto
    {
        public int? Id { get; set; }
    }


    public class ColorFilterDtos
    {
        public ColorFilterDto Filter { get; set; } = new();
        public ColorFilterDto FilterMin { get; set; } = new();
        public ColorFilterDto FilterMax { get; set; } = new();
    }
}
