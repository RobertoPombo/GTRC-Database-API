using Newtonsoft.Json;
using System.Text;

using GTRC_Basics;

namespace GTRC_Database_API.Helpers
{
    public static class Basics
    {
        public static SqlConnectionConfig GetSqlConnection()
        {
            string pathSqlConnectionConfig = GlobalValues.DataDirectory + "config sqlConnection.json";
            if (!Directory.Exists(GlobalValues.DataDirectory)) { Directory.CreateDirectory(GlobalValues.DataDirectory); }
            if (!File.Exists(pathSqlConnectionConfig)) { File.WriteAllText(pathSqlConnectionConfig, JsonConvert.SerializeObject(new SqlConnectionConfig(), Formatting.Indented), Encoding.Unicode); }
            return JsonConvert.DeserializeObject<SqlConnectionConfig>(File.ReadAllText(pathSqlConnectionConfig, Encoding.Unicode)) ?? new();
        }
    }
}
