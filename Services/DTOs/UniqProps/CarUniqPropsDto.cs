using System.ComponentModel.DataAnnotations;

using GTRC_Basics;

namespace GTRC_Database_API.Services.DTOs
{
    public class CarUniqPropsDto0
    {
        [Required] public int AccCarId { get; set; } = GlobalValues.NoID;
    }
}
