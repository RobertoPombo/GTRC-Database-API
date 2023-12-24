using Newtonsoft.Json;
using System.Text;

using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services;
using GTRC_Database_API.Services.DTOs;

namespace GTRC_Database_API.Helpers
{
    public static class Basics
    {
        public static void SetUniqueProperties()
        {
            Dictionary<DtoType, Type> DtoModelsCar = [];
            DtoModelsCar[DtoType.Add] = typeof(CarAddDto);
            DtoModelsCar[DtoType.Update] = typeof(CarUpdateDto);
            DtoModelsCar[DtoType.Filter] = typeof(CarFilterDto);
            DtoModelsCar[DtoType.Filters] = typeof(CarFilterDtos);
            BaseService<Car>.DictDtoModels[typeof(Car)] = DtoModelsCar;
            BaseService<Car>.DictUniqPropsDtoModels[typeof(Car)] = [typeof(CarUniqPropsDto0)];

            Dictionary<DtoType, Type> DtoModelsTrack = [];
            DtoModelsTrack[DtoType.Add] = typeof(TrackAddDto);
            DtoModelsTrack[DtoType.Update] = typeof(TrackUpdateDto);
            DtoModelsTrack[DtoType.Filter] = typeof(TrackFilterDto);
            DtoModelsTrack[DtoType.Filters] = typeof(TrackFilterDtos);
            BaseService<Track>.DictDtoModels[typeof(Track)] = DtoModelsTrack;
            BaseService<Track>.DictUniqPropsDtoModels[typeof(Track)] = [typeof(TrackUniqPropsDto0)];
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
