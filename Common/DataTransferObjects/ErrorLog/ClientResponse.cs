using Common.Constants;

namespace Common.DataTransferObjects.ErrorLog
{
    public class MessageResponse
    {
        public bool IsCompleted { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string Error { get; set; }

    }
}