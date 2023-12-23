using GTRC_Basics.Models.Common;

namespace GTRC_Database_API.Services.DTOs
{
    public abstract class Mapper<ModelType> where ModelType : class, IBaseModel, new()
    {
        public abstract ModelType Map();
        public abstract void Map(ModelType modelType);
    }
}
