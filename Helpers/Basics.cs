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
        public static readonly List<Type> ModelTypeList = [typeof(Color), typeof(Car), typeof(Track), typeof(User), typeof(Series)];

        public static void SetUniqueProperties()
        {
            Dictionary<DtoType, Type> DtoModels = [];
            DtoModels[DtoType.Add] = typeof(ColorAddDto);
            DtoModels[DtoType.Update] = typeof(ColorUpdateDto);
            DtoModels[DtoType.Filter] = typeof(ColorFilterDto);
            DtoModels[DtoType.Filters] = typeof(ColorFilterDtos);
            BaseService<Color>.DictDtoModels[typeof(Color)] = DtoModels;
            BaseService<Color>.DictUniqPropsDtoModels[typeof(Color)] = [typeof(ColorUniqPropsDto0)];

            DtoModels = [];
            DtoModels[DtoType.Add] = typeof(CarAddDto);
            DtoModels[DtoType.Update] = typeof(CarUpdateDto);
            DtoModels[DtoType.Filter] = typeof(CarFilterDto);
            DtoModels[DtoType.Filters] = typeof(CarFilterDtos);
            BaseService<Car>.DictDtoModels[typeof(Car)] = DtoModels;
            BaseService<Car>.DictUniqPropsDtoModels[typeof(Car)] = [typeof(CarUniqPropsDto0)];

            DtoModels = [];
            DtoModels[DtoType.Add] = typeof(TrackAddDto);
            DtoModels[DtoType.Update] = typeof(TrackUpdateDto);
            DtoModels[DtoType.Filter] = typeof(TrackFilterDto);
            DtoModels[DtoType.Filters] = typeof(TrackFilterDtos);
            BaseService<Track>.DictDtoModels[typeof(Track)] = DtoModels;
            BaseService<Track>.DictUniqPropsDtoModels[typeof(Track)] = [typeof(TrackUniqPropsDto0)];

            DtoModels = [];
            DtoModels[DtoType.Add] = typeof(UserAddDto);
            DtoModels[DtoType.Update] = typeof(UserUpdateDto);
            DtoModels[DtoType.Filter] = typeof(UserFilterDto);
            DtoModels[DtoType.Filters] = typeof(UserFilterDtos);
            BaseService<User>.DictDtoModels[typeof(User)] = DtoModels;
            BaseService<User>.DictUniqPropsDtoModels[typeof(User)] = [typeof(UserUniqPropsDto0), typeof(UserUniqPropsDto1)];

            DtoModels = [];
            DtoModels[DtoType.Add] = typeof(SeriesAddDto);
            DtoModels[DtoType.Update] = typeof(SeriesUpdateDto);
            DtoModels[DtoType.Filter] = typeof(SeriesFilterDto);
            DtoModels[DtoType.Filters] = typeof(SeriesFilterDtos);
            BaseService<Series>.DictDtoModels[typeof(Series)] = DtoModels;
            BaseService<Series>.DictUniqPropsDtoModels[typeof(Series)] = [typeof(SeriesUniqPropsDto0)];
        }

        public static SqlConnectionConfig GetSqlConnection()
        {
            string pathSqlConnectionConfig = GlobalValues.DataDirectory + "config sqlConnection.json";
            if (!Directory.Exists(GlobalValues.DataDirectory)) { Directory.CreateDirectory(GlobalValues.DataDirectory); }
            if (!File.Exists(pathSqlConnectionConfig)) { File.WriteAllText(pathSqlConnectionConfig, JsonConvert.SerializeObject(new SqlConnectionConfig(), Formatting.Indented), Encoding.Unicode); }
            return JsonConvert.DeserializeObject<SqlConnectionConfig>(File.ReadAllText(pathSqlConnectionConfig, Encoding.Unicode)) ?? new();
        }

        public static bool IsForeignId(string propertyName)
        {
            if (propertyName.Length > 2 && propertyName[^2..] == "Id")
            {
                foreach (Type type in ModelTypeList)
                {
                    if (propertyName[..^2] == type.Name) { return true; }
                }
                return false;
            }
            return false;
        }
    }
}
