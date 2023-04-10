namespace Common.DataTransferObjects.AppSettings
{
    public class AzureAdClientDefinition
    {
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string Instance { get; set; }
        public string CallbackPath { get; set; }
        public string AccessDeniedPath { get; set; }
    }
}
