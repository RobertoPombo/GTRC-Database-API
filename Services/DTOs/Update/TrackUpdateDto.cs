using System.ComponentModel.DataAnnotations;

using GTRC_Basics;

namespace GTRC_Database_API.Services.DTOs
{
    public class TrackUpdateDto : TrackAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;
    }
}
