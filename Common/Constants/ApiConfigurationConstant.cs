namespace Common.Constants
{
    public static class ApiVersionConstant
    {
        public const string HeaderKey = "X-ApiVersion";
        public const string QueryString = "ApiVersion";
    }

    public static class ApiDocumentConstant
    {
        public const string Bearer = "Bearer";
        public const string OAuth2 = "oauth2";
        public const string Authorization = "Authorization";
        public const string Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"";
    }

    public static class ApiHomePageConstant
    {
        public const string ContentType = "text/html";
        public const string ContentFormat = "Api Name: <strong>{0}</strong><br/>Environment: <strong>{1}</strong><br/>Documentation: <a href='{2}://{3}/docs/index.html'>{2}://{3}/docs/index.html</a><br/>Assembly Version: <strong>{4}</strong>";
    }
}
