using System.ComponentModel.DataAnnotations;

using GTRC_Basics;
using GTRC_Basics.Models;

namespace GTRC_Database_API.Services.DTOs
{
    public class UserUniqPropsDto0 : Mapper<User>
    {
        [Required] public ulong SteamId { get; set; } = User.MinSteamId;
    }

    public class UserUniqPropsDto1 : Mapper<User>
    {
        [Required] public ulong DiscordId { get; set; } = User.NoDiscordId;
    }


    public class UserAddDto : Mapper<User>
    {
        public ulong? SteamId { get; set; }
        public ulong? DiscordId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? BanDate { get; set; }
        public string? Name3Digits { get; set; }
        public short? EloRating { get; set; }
        public short? SafetyRating { get; set; }
        public byte? Warnings { get; set; }
        public string? DiscordName { get; set; }
        public bool? IsOnDiscordServer { get; set; }
        public string? AccessToken { get; set; }
    }


    public class UserUpdateDto : UserAddDto
    {
        [Required] public int Id { get; set; } = GlobalValues.NoID;
    }


    public class UserFilterDto : UserAddDto
    {
        public int? Id { get; set; }
    }


    public class UserFilterDtos
    {
        public UserFilterDto Filter { get; set; } = new();
        public UserFilterDto FilterMin { get; set; } = new();
        public UserFilterDto FilterMax { get; set; } = new();
    }
}
