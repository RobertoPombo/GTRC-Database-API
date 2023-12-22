namespace GTRC_Database_API.Helpers
{
    public class HostBuilder
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
        {
            //webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls("http://localhost:5003", "https://localhost:5004");
        });
    }
}
