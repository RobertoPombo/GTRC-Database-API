using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class CarUpdateDto : CarAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;

        public override Car Map(Car obj) { obj.Id = Id; return base.Map(obj); }

        public override void ReMap(Car obj) { base.ReMap(obj); obj.Id = Id; }
    }
}
