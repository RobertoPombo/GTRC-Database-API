using System.Reflection;

namespace GTRC_Database_API.Models.Common
{
    public interface IBaseModel
    {
        public static List<List<PropertyInfo>> UniqProps = [[]];
        public int Id { get; set; }
    }
}
