namespace Common.DataTransferObjects.AppSettings
{
    public class ClientSetting
    {
        public string CallbackPath { get; set; }
        public string AccessDeniedPath { get; set; }
        public int CacheExpirationMinutes { get; set; }
    }
}
