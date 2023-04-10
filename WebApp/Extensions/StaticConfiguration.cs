namespace WebApp.Extensions
{
    public static class StaticConfiguration
    {
        public static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json");
                IConfiguration configuration = builder.Build();
                return configuration;
            }
        }
    }
}
