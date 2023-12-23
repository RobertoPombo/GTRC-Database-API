using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class CarUpdateDto : CarAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;

        public override Car Map() { Car obj = base.Map(); obj.Id = Id; return obj; }

        public override void Map(Car obj) { base.Map(obj); obj.Id = Id; }
    }
}
