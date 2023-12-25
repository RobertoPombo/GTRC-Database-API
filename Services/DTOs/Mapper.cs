using System.Reflection;
using GTRC_Basics;
using GTRC_Basics.Models.Common;

namespace GTRC_Database_API.Services.DTOs
{
    public abstract class Mapper<ModelType> where ModelType : class, IBaseModel, new()
    {
        public ModelType Map() { return Map(new ModelType()); }

        public ModelType Map(ModelType obj)
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                foreach (PropertyInfo objProperty in obj.GetType().GetProperties())
                {
                    if (property.Name == objProperty.Name && property.GetValue(this) is not null) { objProperty.SetValue(obj, property.GetValue(this)); }
                }
            }
            return obj;
        }

        public void ReMap(ModelType obj)
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                foreach (PropertyInfo objProperty in obj.GetType().GetProperties())
                {
                    if (property.Name == objProperty.Name && objProperty.GetValue(obj) is not null) { property.SetValue(this, objProperty.GetValue(obj)); }
                }
            }
        }

        public bool IsSimilar(ModelType obj)
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                foreach (PropertyInfo objProperty in obj.GetType().GetProperties())
                {
                    if (property.Name == objProperty.Name && property.GetValue(this) is not null && Scripts.GetCastedValue(this, property) != Scripts.GetCastedValue(obj, objProperty)) { return false; }
                }
            }
            return true;
        }
    }
}
