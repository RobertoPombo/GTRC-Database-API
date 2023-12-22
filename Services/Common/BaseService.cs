using System.Reflection;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class BaseService<ModelType>(IBaseContext<ModelType> iBaseContext) where ModelType : class, IBaseModel, new()
    {
        public bool IsUnique(ModelType obj)
        {
            for (int index = 0; index < BaseModel.UniqProps.Count; index++)
            {
                if (!IsUnique(obj, index)) { return false; }
            }
            return true;
        }

        public bool IsUnique(ModelType obj, int index)
        {
            if (BaseModel.UniqProps.Count > index && BaseModel.UniqProps[index].Count > 0)
            {
                int objIndex0 = -1;
                List<ModelType> list = GetAll();
                if (list.Contains(obj)) { objIndex0 = list.IndexOf(obj); }
                for (int objIndex = 0; objIndex < list.Count; objIndex++)
                {
                    if (objIndex != objIndex0)
                    {
                        bool identical = true;
                        foreach (PropertyInfo property in BaseModel.UniqProps[index])
                        {
                            if (Scripts.GetCastedValue(obj!, property) != Scripts.GetCastedValue(list[objIndex]!, property)) { identical = false; break; }
                        }
                        if (identical) { return false; }
                    }
                }
            }
            return true;
        }

        public List<ModelType> GetAll() { return iBaseContext.GetAll().Result; }

        public ModelType? GetById(int id) { return iBaseContext.GetById(id).Result; }

        public ModelType GetNextAvailable() { return new ModelType(); }//return SetNextAvailable(new ModelType()); }//=> Call in CarService erf.
    }
}
