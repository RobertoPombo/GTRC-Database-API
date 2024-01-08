using System.Text.Json.Serialization;

namespace GTRC_Database_API.Helpers
{
    public class SqlConnectionConfig
    {
        private static readonly string defConStr = "Data Source=@\\@,@;Initial Catalog=@;User ID=@;Password=@;Integrated Security=True@;TrustServerCertificate=true";

        public SqlConnectionConfig() { }

        public bool IsLocal { get; set; } = false;
        public string SourceName { get; set; } = string.Empty;
        public string CatalogName { get; set; } = string.Empty;
        public string PCName { get; set; } = string.Empty;
        public string IP6Address { get; set; } = string.Empty;
        public int Port { get; set; } = 1433;
        public string UserID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [JsonIgnore] public string ConnectionString
        {
            get
            {
                string[] _defConStr = defConStr.Split("@");
                _defConStr[1] += SourceName;
                _defConStr[2] += Port.ToString();
                _defConStr[3] += CatalogName;
                _defConStr[4] += UserID;
                _defConStr[5] += Password;
                if (IsLocal)
                {
                    _defConStr[0] += PCName;
                    _defConStr[2] = string.Empty;
                    _defConStr[4] = string.Empty;
                    _defConStr[5] = string.Empty;
                }
                else
                {
                    _defConStr[0] += IP6Address;
                    _defConStr[6] = string.Empty;
                }
                return string.Join(string.Empty, _defConStr);
            }
            set { }
        }
    }
}
