namespace Common.Constants
{
    public static class ErrorMessageTypeConstant
    {
        public const string ArgumentException = "ArgumentException";
        public const string InternalServerException = "InternalServerException";
        public const string BadRequest = "BadRequest";
        public const string NotFound = "NotFound";
    }

    public static class TokenErrorConstant
    {
        public const string TokenRequestError = "Something went wrong while requesting the access token.";
        public const string NotSupportedGrantType = "Not supported grant type.";
    }

    public static class StatusCodeConstant
    {
        public const string Title401 = "Not Authorized";
        public const string SubTitle401 = "{0} is not authorized to access this page or data";
        public const string Message401 = "";
        public const string ImagePath401 = "~/img/error/403.svg";

        public const string Title404 = "Not Found";
        public const string SubTitle404 = "The page you are looking for does not exist";
        public const string Message404 = "";
        public const string ImagePath404 = "~/img/error/404.svg";

        public const string TitleUnknown = "Unknown error";
        public const string SubTitleUnknown = "Unknown error occur";
        public const string MessageUnknown = "";
        public const string ImagePathUnknown = "~/img/error/500.svg";

        public const string Title500 = "IT'S BROKEN! And it's our fault not yours.";
        public const string SubTitle500 = "We'll check our logs with TraceId: {0}";
        public const string Message500 = "";
        public const string ImagePath500 = "~/img/error/500.svg";
    }
}
