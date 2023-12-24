using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class SeriesUniqPropsDto0 : Mapper<Series>
    {
        [Required] public string Name { get; set; } = Series.DefaultName;
    }


    public class SeriesAddDto : Mapper<Series>
    {
        public string? Name { get; set; }
    }


    public class SeriesUpdateDto : SeriesAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;
    }


    public class SeriesFilterDto : SeriesAddDto
    {
        public int? Id { get; set; }
    }


    public class SeriesFilterDtos
    {
        public SeriesFilterDto Filter { get; set; } = new();
        public SeriesFilterDto FilterMin { get; set; } = new();
        public SeriesFilterDto FilterMax { get; set; } = new();
    }
}
