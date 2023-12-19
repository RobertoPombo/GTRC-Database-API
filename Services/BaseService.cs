using System.Reflection;

using GTRC_Basics;
using GTRC_Database_API.Models;

namespace GTRC_Database_API.Services
{
    public abstract class BaseService<ModelType>
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
                List<ModelType> list = GetAllAsync();
                if (list.Contains(obj)) { objIndex0 = list.IndexOf(obj); }
                for (int objIndex = 0; objIndex < list.Count; objIndex++)
                {
                    if (objIndex != objIndex0)
                    {
                        bool identical = true;
                        foreach (PropertyInfo property in BaseModel.UniqProps[index])
                        {
                            if (Scripts.GetCastedValue(this, property) != Scripts.GetCastedValue(list[objIndex], property)) { identical = false; break; }
                        }
                        if (identical) { return false; }
                    }
                }
            }
            return true;
        }

        public abstract ModelType SetNextAvailable(ModelType obj);

        public abstract ModelType Validate(ModelType obj);

        public abstract List<ModelType> GetAllAsync();
    }
}
