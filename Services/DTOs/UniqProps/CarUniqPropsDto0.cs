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
}
