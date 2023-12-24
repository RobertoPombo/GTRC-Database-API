using System.Reflection;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class BaseService<ModelType> where ModelType : class, IBaseModel, new()
    {
        public static readonly Dictionary<Type, Dictionary<DtoType, Type>> DictDtoModels = [];
        public static readonly Dictionary<Type, List<Type>> DictUniqPropsDtoModels = [];
        public readonly Dictionary<DtoType, Type> DtoModels = [];
        public readonly List<Type> UniqPropsDtoModels = [];
        public readonly List<List<PropertyInfo>> UniqProps = [[]];
        public IBaseContext<ModelType> iBaseContext;

        public BaseService(IBaseContext<ModelType> _iBaseContext)
        {
            if (DictDtoModels.ContainsKey(typeof(ModelType))) { DtoModels = DictDtoModels[typeof(ModelType)]; }
            if (DictUniqPropsDtoModels.ContainsKey(typeof(ModelType)))
            {
                UniqProps = [];
                UniqPropsDtoModels = DictUniqPropsDtoModels[typeof(ModelType)];
                foreach (Type uniqPropDto in UniqPropsDtoModels)
                {
                    List<PropertyInfo> propertyList = [];
                    foreach (PropertyInfo property in uniqPropDto.GetProperties())
                    {
                        foreach (PropertyInfo baseProperty in typeof(ModelType).GetProperties())
                        {
                            if (property.Name == baseProperty.Name) { propertyList.Add(baseProperty); }
                        }
                    }
                    UniqProps.Add(propertyList);
                }
            }
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

        public async Task<ModelType?> GetByUniqProps(dynamic objDto, int index = 0)
        {
            if (UniqProps.Count > index && UniqProps[index].Count > 0 && UniqProps[index].Count == Scripts.GetPropertyList(objDto.GetType()).Count)
            {
                List<ModelType> list = await GetByProps(objDto, true, index);
                if (list.Count == 1) { return list[0]; }
                else { return null; }
            }
            return null;
        }

        public async Task<List<ModelType>> GetByProps(dynamic objDto, bool firstOnly = false, int indexUniqProps = -1)
        {
            List<ModelType> _list = [];
            List<ModelType> list = await GetAll();
            List<PropertyInfo> listProps = Scripts.GetPropertyList(typeof(ModelType));
            if (firstOnly && indexUniqProps >= 0 && indexUniqProps < UniqProps.Count) { listProps = UniqProps[indexUniqProps]; }
            foreach (ModelType obj in list)
            {
                bool found = true;
                foreach (PropertyInfo property in listProps)
                {
                    foreach (PropertyInfo filterProperty in objDto.GetType().GetProperties())
                    {
                        if (filterProperty.Name == property.Name && filterProperty.GetValue(objDto) is not null && property.GetValue(obj) is not null)
                        {
                            if (Scripts.GetCastedValue(obj, property) != Scripts.GetCastedValue(objDto, filterProperty))
                            {
                                found = false;
                                break;
                            }
                            break;
                        }
                    }
                }
                if (found) { _list.Add(obj); if (firstOnly) { return _list; } }
            }
            return _list;
        }

        public async Task<List<ModelType>> GetByFilter(dynamic objFilter, dynamic objFilterMin, dynamic objFilterMax)
        {
            List<ModelType> list = await GetAll();
            List<ModelType> filteredList = [];
            List<PropertyInfo> listModelProps = Scripts.GetPropertyList(typeof(ModelType));
            List<PropertyInfo> listFilterProps = Scripts.GetPropertyList(objFilter.GetType());
            foreach (ModelType obj in list)
            {
                bool isInList = true;
                foreach (PropertyInfo filterProperty in listFilterProps)
                {
                    var filter = filterProperty.GetValue(objFilter);
                    var filterMin = filterProperty.GetValue(objFilterMin);
                    var filterMax = filterProperty.GetValue(objFilterMax);
                    if (filter is not null || filterMin is not null || filterMax is not null)
                    {
                        string strFilter = filter?.ToString()?.ToLower() ?? "";
                        string strFilterMin = filterMin?.ToString()?.ToLower() ?? "";
                        string strFilterMax = filterMax?.ToString()?.ToLower() ?? "";
                        foreach (PropertyInfo property in listModelProps)
                        {
                            if (filterProperty.Name == property.Name)
                            {
                                var castedValue = Scripts.GetCastedValue(obj, property);
                                string strValue = castedValue.ToString().ToLower();
                                if (!strValue.Contains(strFilter)) { isInList = false; }
                                else if (GlobalValues.numericalTypes.Contains(property.PropertyType.ToString()))
                                {
                                    if (filterMin is not null)
                                    {
                                        var castedFilterMin = Scripts.CastValue(property, filterMin);
                                        if (castedValue < castedFilterMin) { isInList = false; }
                                    }
                                    if (filterMax is not null)
                                    {
                                        var castedFilterMax = Scripts.CastValue(property, filterMax);
                                        if (castedValue > castedFilterMax) { isInList = false; }
                                    }
                                }
                                else if ((strFilterMin.Length > 0 && string.Compare(strValue, strFilterMin) == -1)
                                    || (strFilterMax.Length > 0 && string.Compare(strValue, strFilterMax) == 1))
                                {
                                    isInList = false;
                                }
                                break;
                            }
                        }
                        if (!isInList) { break; }
                    }
                }
                if (isInList) { filteredList.Add(obj); }
            }
            return filteredList;
        }

        public async Task SaveChanges() { await iBaseContext.SaveChanges(); }

        public async Task<List<ModelType>> GetAll() { return await iBaseContext.GetAll(); }

        public async Task<ModelType?> GetById(int id) { return await iBaseContext.GetById(id); }

        public async Task Add(ModelType obj) { await iBaseContext.Add(obj); }

        public async Task Delete(ModelType obj) { await iBaseContext.Delete(obj); }

        public async Task Update(ModelType obj) { await iBaseContext.Update(obj); }
    }
}
