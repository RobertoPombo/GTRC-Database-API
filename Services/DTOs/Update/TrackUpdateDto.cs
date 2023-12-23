using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class TrackUpdateDto : TrackAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;

        public override Track Map(Track obj) { obj.Id = Id; return base.Map(obj); }

        public override void ReMap(Track obj) { base.ReMap(obj); obj.Id = Id; }
    }
}
