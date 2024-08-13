using GTRC_Basics;
using GTRC_Basics.Models;
using GTRC_Database_API.Services.Interfaces;

namespace GTRC_Database_API.Services
{
    public class SeriesDiscordchanneltypeService(ISeriesDiscordchanneltypeContext iSeriesDiscordchanneltypeContext,
        IBaseContext<Series> iSeriesContext,
        IBaseContext<SeriesDiscordchanneltype> iBaseContext) : BaseService<SeriesDiscordchanneltype>(iBaseContext)
    {
        public bool Validate(SeriesDiscordchanneltype? obj)
        {
            bool isValid = true;
            if (obj is null) { return false; }

            return isValid;
        }

        public async Task<bool> ValidateUniqProps(SeriesDiscordchanneltype? obj)
        {
            bool isValidUniqProps = true;
            if (obj is null) { return false; }

            Series? series = null;
            if (obj.Series is not null) { series = iSeriesContext.GetById(obj.SeriesId).Result; };
            if (series is null)
            {
                List<Series> list = iSeriesContext.GetAll().Result;
                if (list.Count == 0) { obj = null; return false; }
                else { obj.Series = list[0]; obj.SeriesId = list[0].Id; isValidUniqProps = false; }
            }
            else { obj.Series = series; }
            if (!Scripts.IsValidDiscordId(obj.DiscordId)) { obj.DiscordId = GlobalValues.MinDiscordId; isValidUniqProps = false; }

            int startIndexSeries = 0;
            List<int> idListSeries = [];
            List<Series> listSeries = iSeriesContext.GetAll().Result;
            for (int index = 0; index < listSeries.Count; index++)
            {
                idListSeries.Add(listSeries[index].Id);
                if (listSeries[index].Id == obj.SeriesId) { startIndexSeries = index; }
            }
            int indexSeries = startIndexSeries;

            DiscordChannelType[] listTypes = (DiscordChannelType[])Enum.GetValues(typeof(DiscordChannelType));
            if (listTypes.Length == 0) { obj = null; return false; }
            int startIndexType = 0;
            for (int _indexType = 0; _indexType < listTypes.Length; _indexType++) { if (obj.DiscordChannelType == listTypes[_indexType]) { startIndexType = _indexType; break; } }
            int indexType = startIndexType;
            obj.DiscordChannelType = listTypes[indexType];
            while (!await IsUnique(obj, 0))
            {
                isValidUniqProps = false;
                if (indexType < listTypes.Length - 1) { indexType++; }
                else { indexType = 0; }
                obj.DiscordChannelType = listTypes[indexType];
                if (indexType == startIndexType)
                {
                    if (indexSeries < idListSeries.Count - 1) { indexSeries++; }
                    else { indexSeries = 0; }
                    obj.Series = listSeries[indexSeries];
                    obj.SeriesId = listSeries[indexSeries].Id;
                    if (indexSeries == startIndexSeries) { obj = null; return false; }
                }
            }

            ulong startValue = obj.DiscordId;
            while (!await IsUnique(obj, 1))
            {
                isValidUniqProps = false;
                if (obj.DiscordId < GlobalValues.MaxDiscordId) { obj.DiscordId++; } else { obj.DiscordId = GlobalValues.MinDiscordId; }
                if (obj.DiscordId == startValue) { obj = null; return false; }
            }

            Validate(obj);
            return isValidUniqProps;
        }

        public async Task<SeriesDiscordchanneltype?> GetTemp() { SeriesDiscordchanneltype obj = new(); await ValidateUniqProps(obj); return obj; }
    }
}
