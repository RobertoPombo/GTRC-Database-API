using GTRC_Basics.Models.Common;

namespace GTRC_Database_API.Services.DTOs
{
    public abstract class Mapper<ModelType> where ModelType : class, IBaseModel, new()
    {
        public ModelType Map() { return Map(new ModelType()); }
        public abstract ModelType Map(ModelType modelType);
        public abstract void ReMap(ModelType modelType);
    }
}
