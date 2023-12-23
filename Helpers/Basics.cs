using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using Newtonsoft.Json;
using System.Text;

namespace GTRC_Database_API.Helpers
{
    public static class Basics
    {
        public static void SetUniqueProperties()
        {
            BaseService<Car>.ListUniqProps[typeof(Car)] = [[typeof(Car).GetProperty(nameof(Car.AccCarId))!]];
            BaseService<Track>.ListUniqProps[typeof(Track)] = [[typeof(Track).GetProperty(nameof(Track.AccTrackId))!]];
        }

        public static SqlConnectionConfig GetSqlConnection()
        {
            string pathSqlConnectionConfig = GlobalValues.DataDirectory + "config sqlConnection.json";
            if (!Directory.Exists(GlobalValues.DataDirectory)) { Directory.CreateDirectory(GlobalValues.DataDirectory); }
            if (!File.Exists(pathSqlConnectionConfig)) { File.WriteAllText(pathSqlConnectionConfig, JsonConvert.SerializeObject(new SqlConnectionConfig(), Formatting.Indented), Encoding.Unicode); }
            return JsonConvert.DeserializeObject<SqlConnectionConfig>(File.ReadAllText(pathSqlConnectionConfig, Encoding.Unicode)) ?? new();
        }
    }
}
