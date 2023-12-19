using System.Reflection;

using GTRC_Basics;

namespace GTRC_Database_API.Models
{
    public abstract class BaseModel
    {
        public static List<List<PropertyInfo>> UniqProps = [[]];

        public int Id { get; set; } = GlobalValues.NoID;
    }
}
