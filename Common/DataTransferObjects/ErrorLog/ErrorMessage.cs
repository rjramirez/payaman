using Common.Constants;

namespace Common.DataTransferObjects.ErrorLog
{
    public class ErrorMessage
    {
        public string TraceId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }

        public ErrorMessage()
        {

        }

        public ErrorMessage(string traceId, string type, string message)
        {
            TraceId = traceId;
            Type = type;
            Message = message;
        }

        public ErrorMessage(string type, string message)
        {
            Type = type;
            Message = message;
        }

        public ErrorMessage(string message)
        {
            Type = ErrorMessageTypeConstant.BadRequest;
            Message = message;
        }
    }
}