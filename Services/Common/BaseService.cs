using System.Collections.Generic;
using System.Data;
using System.Reflection;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Database_API.Helpers;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class BaseService<ModelType> where ModelType : class, IBaseModel, new()
    {
        public static readonly Dictionary<Type, List<List<PropertyInfo>>> ListUniqProps = [];
        public readonly List<List<PropertyInfo>> UniqProps = [[]];
        public IBaseContext<ModelType> iBaseContext;

        public BaseService(IBaseContext<ModelType> _iBaseContext)
        {
            if (ListUniqProps.ContainsKey(typeof(ModelType))) { UniqProps = ListUniqProps[typeof(ModelType)]; }
            iBaseContext = _iBaseContext;
        }

        public async Task<bool> IsUnique(ModelType obj)
        {
            for (int index = 0; index < UniqProps.Count; index++)
            {
                if (!await IsUnique(obj, index)) { return false; }
            }
            return true;
        }

        public async Task<bool> IsUnique(ModelType obj, int index)
        {
            if (UniqProps.Count > index && UniqProps[index].Count > 0)
            {
                int objIndex0 = -1;
                List<ModelType> list = await GetAll();
                if (list.Contains(obj)) { objIndex0 = list.IndexOf(obj); }
                for (int objIndex = 0; objIndex < list.Count; objIndex++)
                {
                    if (objIndex != objIndex0)
                    {
                        bool identical = true;
                        foreach (PropertyInfo property in UniqProps[index])
                        {
                            if (Scripts.GetCastedValue(obj!, property) != Scripts.GetCastedValue(list[objIndex]!, property)) { identical = false; break; }
                        }
                        if (identical) { return false; }
                    }
                }
            }
            return true;
        }

        public async Task<ModelType?> GetByUniqProp(dynamic _value, int index = 0)
        {
            if (UniqProps.Count > index && UniqProps[index].Count == 1)
            {
                return await GetByUniqProps([_value], index);
            }
            return null;
        }

        public async Task<ModelType?> GetByUniqProps(List<dynamic> values, int index = 0)
        {
            if (UniqProps.Count > index && UniqProps[index].Count > 0 && UniqProps[index].Count == values.Count)
            {
                List<ModelType> list = await GetBy(UniqProps[index], values, true);
                if (list.Count == 1) { return list[0]; }
                else { return null; }
            }
            return null;
        }

        public async Task<List<ModelType>> GetBy(PropertyInfo property, dynamic _value, bool firstOnly = false)
        {
            return await GetBy([property], [_value], firstOnly);
        }

        public async Task<List<ModelType>> GetBy(List<PropertyInfo> properties, List<dynamic> values, bool firstOnly = false)
        {
            List<ModelType> _list = [];
            List<ModelType> list = await GetAll();
            if (properties.Count > 0 && properties.Count == values.Count)
            {
                foreach (ModelType _obj in list)
                {
                    bool found = true;
                    for (int propertyNr = 0; propertyNr < properties.Count; propertyNr++)
                    {
                        if (Scripts.GetCastedValue(_obj, properties[propertyNr]) != Scripts.CastValue(properties[propertyNr], values[propertyNr]))
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found) { _list.Add(_obj); if (firstOnly) { return _list; } }
                }
            }
            return _list;
        }

        public async Task SaveChanges() { await iBaseContext.SaveChanges(); }

        public async Task<List<ModelType>> GetAll() { return await iBaseContext.GetAll(); }

        public async Task<ModelType?> GetById(int id) { return await iBaseContext.GetById(id); }

        public async Task Add(ModelType obj) { await iBaseContext.Add(obj); }

        public async Task Delete(ModelType obj) { await iBaseContext.Delete(obj); }

        public async Task Update(ModelType obj) { await iBaseContext.Update(obj); }
    }
}
