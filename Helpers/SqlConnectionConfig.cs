namespace GTRC_Database_API.Helpers
{
    public class SqlConnectionConfig
    {
        private static readonly string defConStr = "Data Source=@\\@,@;Initial Catalog=@;User ID=@;Password=@;Integrated Security=True;TrustServerCertificate=true";

        public SqlConnectionConfig() { }

        public bool IsLocal { get; set; } = false;
        public string SourceName { get; set; } = "";
        public string CatalogName { get; set; } = "";
        public string PCName { get; set; } = "";
        public string IP6Address { get; set; } = "";
        public int Port { get; set; } = 1433;
        public string UserID { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConnectionString
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
                    _defConStr[2] = "";
                    _defConStr[4] = "";
                    _defConStr[5] = "";
                }
                else
                {
                    _defConStr[0] += IP6Address;
                    _defConStr[6] = "";
                }
                return string.Join("", _defConStr);
            }
            set { }
        }
    }
}
